using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public interface IOnOrderPaid : IEvent<OrderPaidArgs>
    {
    }

    public class OrderPaidArgs
    {
        public Order Order { get; set; }
    }
}