using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderShippingService : IOrderShippingService
    {
        private readonly ISession _session;
        private readonly IBulkShippingUpdateValidationService _bulkShippingUpdateValidationService;
        private readonly IBulkShippingUpdateService _bulkShippingUpdateService;

        public OrderShippingService(ISession session,IBulkShippingUpdateValidationService bulkShippingUpdateValidationService,
                                   IBulkShippingUpdateService bulkShippingUpdateService)
        {
            _session = session;
            _bulkShippingUpdateValidationService = bulkShippingUpdateValidationService;
            _bulkShippingUpdateService = bulkShippingUpdateService;
        }

        public Dictionary<string, List<string>> BulkShippingUpdate(Stream file)
        {
            Dictionary<string, List<string>> parseErrors;
            var items = GetOrdersFromFile(file, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _bulkShippingUpdateValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            var noOfUpdatedItems = _bulkShippingUpdateService.BulkShippingUpdateFromDTOs(items);
            return new Dictionary<string, List<string>>() { { "success", new List<string>() 
            { noOfUpdatedItems > 0 ? noOfUpdatedItems.ToString() + (noOfUpdatedItems > 1? " items were":" item was") +" successfully updated." : "No items were updated." } } };
        }

        private List<BulkShippingUpdateDataTransferObject> GetOrdersFromFile(Stream file, out Dictionary<string, List<string>> parseErrors)
        {
            parseErrors = new Dictionary<string, List<string>>();
            return _bulkShippingUpdateValidationService.ValidateAndBulkShippingUpdateOrders(file, ref parseErrors);
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
                    .SelectMany(s => s.GroupBy(sc => sc.ShippingMethod).Select(sc2 => sc2.OrderBy(sc3 => sc3.GetPrice(cart)).First()))
                    .ToList();
            return calculations;
        }

        public IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart)
        {
            return GetShippingCalculations(cart).GroupBy(x => x.Country).Select(s => s.OrderBy(calculation => calculation.GetPrice(cart)).First()).ToList();
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