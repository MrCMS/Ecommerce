using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
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
        ShippingMethod GetDefaultShippingMethod(CartModel cart);
        IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart);
    }

    public class OrderShippingService : IOrderShippingService
    {
        private readonly ISession _session;

        public OrderShippingService(ISession session)
        {
            _session = session;
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

        public IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart)
        {
            return GetShippingCalculations(cart).GroupBy(x => x.Country).Select(s => s.OrderBy(calculation => calculation.GetPrice(cart)).First()).ToList();
        }

        public ShippingMethod GetDefaultShippingMethod(CartModel cart)
        {
            var firstOrDefault = GetShippingCalculations(cart).FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.ShippingMethod : null;
        }
    }
}
