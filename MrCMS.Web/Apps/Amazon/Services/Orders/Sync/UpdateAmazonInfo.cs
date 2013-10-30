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
        private readonly IEnumerable<IOnAmazonOrderPlaced> _onAmazonOrderPlaceds;
        private readonly IOrderService _orderService;

        public UpdateAmazonInfo(IValidateAmazonOrderService validateAmazonOrderService,
            IEnumerable<IOnAmazonOrderPlaced> onAmazonOrderPlaceds, IOrderService orderService)
        {
            _validateAmazonOrderService = validateAmazonOrderService;
            _onAmazonOrderPlaceds = onAmazonOrderPlaceds;
            _orderService = orderService;
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
            if (amazonOrder.Status == AmazonOrderStatus.Shipped && amazonOrder.Order != null && amazonOrder.Order.ShippingStatus == ShippingStatus.Unshipped)
            {
                _orderService.MarkAsShipped(amazonOrder.Order);
            }
        }

        public int Order { get { return -10; } }
    }
}