using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderPartiallyRefunded : IOrderEvent
    {
        void OnOrderPartiallyRefunded(Order order, OrderRefund refund);
    }
}