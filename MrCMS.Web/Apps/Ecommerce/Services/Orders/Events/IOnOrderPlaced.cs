using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderPlaced : IOrderEvent
    {
        void OnOrderPlaced(Order order);
    }
}