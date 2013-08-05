using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderShipped : IOrderEvent
    {
        void OnOrderShipped(Order order);
    }
}