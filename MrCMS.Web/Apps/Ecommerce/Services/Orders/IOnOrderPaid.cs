using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOnOrderPaid : IOrderEvent
    {
        void OnOrderPaid(Order order);
    }
}