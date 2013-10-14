using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderRefundService : IOrderRefundService
    {
        private readonly ISession _session;
        private readonly IOrderEventService _orderEventService;

        public OrderRefundService(ISession session, IOrderEventService orderEventService)
        {
            _session = session;
            _orderEventService = orderEventService;
        }

        public IList<OrderRefund> GetAll()
        {
            return _session.QueryOver<OrderRefund>().Cacheable().List();
        }

        public void Add(OrderRefund orderRefund)
        {
            orderRefund.Order.OrderRefunds.Add(orderRefund);//cache
            _session.Transact(session => session.Save(orderRefund));

            if (orderRefund.Amount == orderRefund.Order.Total)
                _orderEventService.OrderFullyRefunded(orderRefund.Order, orderRefund);
            else
                _orderEventService.OrderPartiallyRefunded(orderRefund.Order, orderRefund);
        }

        public void Delete(OrderRefund item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}