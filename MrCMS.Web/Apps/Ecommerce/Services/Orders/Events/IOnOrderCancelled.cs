using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderCancelled : IEvent<OrderCancelledArgs>
    {
    }

    public class OrderCancelledArgs
    {
        public Order Order { get; set; }
    }
}