using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderVoided : IEvent<OrderVoidedArgs>
    {
    }

    public class OrderVoidedArgs
    {
        public Order Order { get; set; }
    }
}