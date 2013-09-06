using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingMethodManager
    {
        IList<ShippingMethod> GetAll();
        ShippingMethod Get(int id);
        List<SelectListItem> GetOptions();
        void Add(ShippingMethod ShippingMethod);
        void Update(ShippingMethod ShippingMethod);
        void Delete(ShippingMethod ShippingMethod);
        void UpdateDisplayOrder(IList<SortItem> options);
    }

    public interface IOrderShippingService
    {
        List<SelectListItem> GetShippingOptions(CartModel cart);
        List<SelectListItem> GetCheapestShippingOptions(CartModel cart);
        ShippingMethod GetDefaultShippingMethod(CartModel cart);
        IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart);
        List<SelectListItem> ExistingAddressOptions(CartModel cartModel, Address address);
    }

    public class OrderShippingService : IOrderShippingService
    {
        private readonly ISession _session;
        private readonly IUserService _userService;

        public OrderShippingService(ISession session, IUserService userService)
        {
            _session = session;
            _userService = userService;
        }

        public List<SelectListItem> GetShippingOptions(CartModel cart)
        {
            var shippingCalculations = GetShippingCalculations(cart);
            return shippingCalculations.BuildSelectItemList(
                calculation =>
                string.Format("{0} - {1}, {2}", calculation.Country.Name, calculation.ShippingMethod.Name,
                              calculation.GetPrice(cart).Value.ToCurrencyFormat()),
                calculation => calculation.Id.ToString(),
                calculation =>
                cart.ShippingMethod != null && calculation.Country == cart.Country &&
                calculation.ShippingMethod == cart.ShippingMethod,
                emptyItemText: null);
        }

        private IEnumerable<ShippingCalculation> GetShippingCalculations(CartModel cart)
        {
            var shippingCalculations =
                _session.QueryOver<ShippingCalculation>()
                        .Fetch(calculation => calculation.ShippingMethod)
                        .Eager.Cacheable()
                        .List().Where(x => x.CanBeUsed(cart))
                                       .OrderBy(x => x.Country.DisplayOrder)
                                       .ThenBy(x => x.ShippingMethod.DisplayOrder)
                                       .Where(calculation => calculation.GetPrice(cart).HasValue);
            return shippingCalculations;
        }

        private IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountryAndMethod(CartModel cart)
        {
            var calculations =
                GetShippingCalculations(cart)
                    .GroupBy(x => x.Country)
                    .SelectMany(s=>s.GroupBy(sc=>sc.ShippingMethod).Select(sc2=>sc2.OrderBy(sc3=>sc3.GetPrice(cart)).First()))
                    .ToList();
            return calculations;
        }

        public IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart)
        {
            return GetShippingCalculations(cart).GroupBy(x => x.Country).Select(s => s.OrderBy(calculation => calculation.GetPrice(cart)).First()).ToList();
        }

        public List<SelectListItem> ExistingAddressOptions(CartModel cartModel, Address address)
        {
            var addresses = new List<Address>();
            addresses.Add(cartModel.ShippingAddress);
            addresses.Add(cartModel.BillingAddress);

            var currentUser = CurrentRequestData.CurrentUser;
            if (currentUser != null)
                addresses.AddRange(_userService.GetAll<Address>(currentUser));

            addresses = addresses.Distinct().Where(a => !AddressComparison.Comparer.Equals(a, address)).ToList();

            return addresses.Any()
                       ? addresses.BuildSelectItemList(a => a.GetDescription(), a => a.ToJSON(),
                                                       emptyItemText: "Select an address...")
                       : new List<SelectListItem>();

        }

        public List<SelectListItem> GetCheapestShippingOptions(CartModel cart)
        {
            var shippingCalculations = GetCheapestShippingCalculationsForEveryCountryAndMethod(cart);
            return shippingCalculations.BuildSelectItemList(
                calculation =>
                string.Format("{0} - {1}, {2}", calculation.Country.Name, calculation.ShippingMethod.Name,
                              calculation.GetPrice(cart).Value.ToCurrencyFormat()),
                calculation => calculation.Id.ToString(),
                calculation =>
                cart.ShippingMethod != null && calculation.Country == cart.Country &&
                calculation.ShippingMethod == cart.ShippingMethod,
                emptyItemText: null);
        }

        public ShippingMethod GetDefaultShippingMethod(CartModel cart)
        {
            var firstOrDefault = GetShippingCalculations(cart).FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.ShippingMethod : null;
        }
    }
}
