using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderEventService
    {
        void OrderPlaced(Order order);
        void OrderCancelled(Order order);
        void OrderShipped(Order order);
        void OrderFullyRefunded(Order order, OrderRefund refund);
        void OrderPartiallyRefunded(Order order, OrderRefund refund);
    }
}