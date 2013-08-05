using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderFullyRefunded : IOrderEvent
    {
        void OnOrderFullyRefunded(Order order, OrderRefund refund);
    }
}