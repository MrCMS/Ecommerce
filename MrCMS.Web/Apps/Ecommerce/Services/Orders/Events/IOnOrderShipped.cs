using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderShipped : IEvent<OrderShippedArgs>
    {
    }

    public class OrderShippedArgs
    {
        public Order Order { get; set; }
    }
}