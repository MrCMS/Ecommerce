using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderPlaced : IEvent<OrderPlacedArgs>
    {
    }

    public class OrderPlacedArgs
    {
        public Order Order { get; set; }
    }
}