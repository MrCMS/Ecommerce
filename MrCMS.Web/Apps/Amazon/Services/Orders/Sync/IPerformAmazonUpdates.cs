using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IPerformAmazonUpdates
    {
        void Update(AmazonOrder amazonOrder, Order order);
        int Order { get; }
    }

    public class CancelAmazonOrder : IPerformAmazonUpdates
    {
        private readonly IOrderService _orderService;
        private readonly IAmazonOrderService _amazonOrderService;

        public CancelAmazonOrder(IOrderService orderService, IAmazonOrderService amazonOrderService)
        {
            _orderService = orderService;
            _amazonOrderService = amazonOrderService;
        }

        public void Update(AmazonOrder amazonOrder, Order order)
        {
            if (amazonOrder.Status == AmazonOrderStatus.Unshipped && order.OrderStatus == OrderStatusEnum.Canceled)
            {
                amazonOrder.Status = AmazonOrderStatus.Canceled;
                _amazonOrderService.Update(amazonOrder);

                _orderService.Cancel(amazonOrder.Order);
            }
        }

        public int Order { get; private set; }
    }
}