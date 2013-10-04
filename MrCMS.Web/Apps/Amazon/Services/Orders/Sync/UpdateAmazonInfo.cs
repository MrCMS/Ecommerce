using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class UpdateAmazonInfo : IPerformAmazonUpdates
    {
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public UpdateAmazonInfo(IValidateAmazonOrderService validateAmazonOrderService)
        {
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public void Update(AmazonOrder amazonOrder, Order order)
        {
            if (amazonOrder.Id == 0 || (amazonOrder.Status != AmazonOrderStatus.Shipped &&
                                        amazonOrder.Status != AmazonOrderStatus.Canceled))
            {
                var shippingAddress = _validateAmazonOrderService.GetAmazonOrderAddress(order);
                _validateAmazonOrderService.GetAmazonOrderDetails(order, ref amazonOrder, shippingAddress);
            }

            amazonOrder.NumberOfItemsShipped = order.NumberOfItemsShipped;
            amazonOrder.Status = order.OrderStatus.GetEnumByValue<AmazonOrderStatus>();
        }

        public int Order { get { return -10; } }
    }
}