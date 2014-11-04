using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderFullyRefunded : IEvent<OrderFullyRefundedArgs>
    {
    }

    public class OrderFullyRefundedArgs
    {
        public Order Order { get; set; }
        public OrderRefund Refund { get; set; }
    }
}