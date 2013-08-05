using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using Ninject;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderEventService : IOrderEventService
    {
        private readonly IKernel _kernel;

        public OrderEventService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void OrderPlaced(Order order)
        {
            GetAll<IOnOrderPlaced>().ForEach(placed => placed.OnOrderPlaced(order));
        }

        public void OrderCancelled(Order order)
        {
            GetAll<IOnOrderCancelled>().ForEach(placed => placed.OnOrderCancelled(order));
        }

        public void OrderShipped(Order order)
        {
            GetAll<IOnOrderShipped>().ForEach(placed => placed.OnOrderShipped(order));
        }

        public void OrderFullyRefunded(Order order, OrderRefund refund)
        {
            GetAll<IOnOrderFullyRefunded>().ForEach(refunded => refunded.OnOrderFullyRefunded(order, refund));
        }

        public void OrderPartiallyRefunded(Order order, OrderRefund refund)
        {
            GetAll<IOnOrderPartiallyRefunded>().ForEach(refunded => refunded.OnOrderPartiallyRefunded(order, refund));
        }

        private IOrderedEnumerable<T> GetAll<T>() where T : IOrderEvent
        {
            return _kernel.GetAll<T>().OrderBy(placed => placed.Order);
        }
    }
}