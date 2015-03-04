using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class UpdateAmazonInfo : IPerformAmazonUpdates
    {
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public UpdateAmazonInfo(IValidateAmazonOrderService validateAmazonOrderService)

        {
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public bool Update(AmazonOrder amazonOrder, Order order)
        {
            if (amazonOrder.Id == 0 || (amazonOrder.Status != AmazonOrderStatus.Shipped &&
                                        amazonOrder.Status != AmazonOrderStatus.Canceled))
            {
                var shippingAddress = _validateAmazonOrderService.GetAmazonOrderAddress(order);
                _validateAmazonOrderService.GetAmazonOrderDetails(order, ref amazonOrder, shippingAddress);
            }

            amazonOrder.NumberOfItemsShipped = order.NumberOfItemsShipped;
            amazonOrder.Status = order.OrderStatus.GetEnumByValue<AmazonOrderStatus>();
            return true;
        }

        public int Order { get { return -10; } }
    }
}