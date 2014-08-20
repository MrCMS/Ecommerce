using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderTrackingNumberChanged : IEvent<OrderTrackingNumberChangedArgs>
    {
    }

    public class OrderTrackingNumberChangedArgs
    {
        public Order Order { get; set; }
    }
}