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
            var items = GetAll<IOnOrderPlaced>();
            if (items.Any())
                items.ForEach(placed => placed.OnOrderPlaced(order));
        }

        public void OrderCancelled(Order order)
        {
            var items = GetAll<IOnOrderCancelled>();
            if (items.Any())
                items.ForEach(placed => placed.OnOrderCancelled(order));
        }

        public void OrderShipped(Order order)
        {
            var items = GetAll<IOnOrderShipped>();
            if(items.Any())
                items.ForEach(placed => placed.OnOrderShipped(order));
        }

        public void OrderFullyRefunded(Order order, OrderRefund refund)
        {
            var items = GetAll<IOnOrderFullyRefunded>();
            if (items.Any())
                items.ForEach(placed => placed.OnOrderFullyRefunded(order, refund));
        }

        public void OrderPartiallyRefunded(Order order, OrderRefund refund)
        {
            var items = GetAll<IOnOrderPartiallyRefunded>();
            if (items.Any())
                items.ForEach(placed => placed.OnOrderPartiallyRefunded(order, refund));
        }

        public void OrderPaid(Order order)
        {
            var items = GetAll<IOnOrderPaid>();
            if (items.Any())
                items.ForEach(placed => placed.OnOrderPaid(order));
        }

        private IOrderedEnumerable<T> GetAll<T>() where T : IOrderEvent
        {
            return _kernel.GetAll<T>().OrderBy(placed => placed.Order);
        }
    }
}