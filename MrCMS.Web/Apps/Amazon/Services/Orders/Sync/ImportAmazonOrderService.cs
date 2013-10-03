using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ImportAmazonOrderService : IImportAmazonOrderService
    {
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public ImportAmazonOrderService(IAmazonOrderService amazonOrderService,
             EcommerceSettings ecommerceSettings, IValidateAmazonOrderService validateAmazonOrderService)
        {
            _amazonOrderService = amazonOrderService;
            _ecommerceSettings = ecommerceSettings;
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public AmazonOrder SetAmazonOrderItems(Order rawOrder, 
            IEnumerable<OrderItem> rawOrderItems, AmazonOrder amazonOrder)
        {
            foreach (var rawOrderItem in rawOrderItems)
                _validateAmazonOrderService.SetAmazonOrderItem(ref amazonOrder, rawOrderItem);

            amazonOrder.Order = _validateAmazonOrderService.GetOrder(rawOrder, amazonOrder);

            return amazonOrder;
        }

        public AmazonOrder GetAmazonOrder(Order rawOrder)
        {
            if (rawOrder.OrderTotal!=null && rawOrder.OrderTotal.CurrencyCode != _ecommerceSettings.Currency.Code) return null;

            var order = _amazonOrderService.GetByAmazonOrderId(rawOrder.AmazonOrderId) ?? new AmazonOrder();

            if (order.Id == 0)
            {
                var shippingAddress = _validateAmazonOrderService.GetAmazonOrderAddress(rawOrder);
                _validateAmazonOrderService.GetAmazonOrderDetails(rawOrder, ref order, shippingAddress);
            }

            order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
            order.Status = rawOrder.OrderStatus.GetEnumByValue<AmazonOrderStatus>();

            return order;
        }
    }
}