using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class CancelAmazonOrder : IPerformAmazonUpdates
    {
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly IOrderAdminService _orderService;

        public CancelAmazonOrder(IOrderAdminService orderService, IAmazonOrderService amazonOrderService)
        {
            _orderService = orderService;
            _amazonOrderService = amazonOrderService;
        }

        public bool Update(AmazonOrder amazonOrder, Order order)
        {
            if (amazonOrder.Status == AmazonOrderStatus.Unshipped && order.OrderStatus == OrderStatusEnum.Canceled)
            {
                amazonOrder.Status = AmazonOrderStatus.Canceled;
                _amazonOrderService.Update(amazonOrder);

                _orderService.Cancel(amazonOrder.Order);
            }
            return true;
        }

        public int Order { get; private set; }
    }
}