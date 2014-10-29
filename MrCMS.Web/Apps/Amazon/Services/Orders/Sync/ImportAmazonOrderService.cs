using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ImportAmazonOrderService : IImportAmazonOrderService
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public ImportAmazonOrderService(EcommerceSettings ecommerceSettings, IValidateAmazonOrderService validateAmazonOrderService)
        {
            _ecommerceSettings = ecommerceSettings;
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public AmazonOrder SetAmazonOrderItems(IEnumerable<OrderItem> rawOrderItems, AmazonOrder amazonOrder)
        {
            foreach (var rawOrderItem in rawOrderItems)
                _validateAmazonOrderService.SetAmazonOrderItem(ref amazonOrder, rawOrderItem);

            amazonOrder.Order = _validateAmazonOrderService.GetOrder(amazonOrder);

            return amazonOrder;
        }

        public bool IsCurrencyValid(Order order)
        {
            var currency = _ecommerceSettings.Currency();
            return !(order.OrderTotal != null && order.OrderTotal.CurrencyCode != currency.Code);
        }
    }
}