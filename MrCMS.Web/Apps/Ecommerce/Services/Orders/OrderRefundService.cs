using System.Collections.Generic;
using MrCMS.Helpers;
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

        public void Add(OrderRefund orderRefund, Order order)
        {
            _session.Transact(session => session.Save(order));
            if(orderRefund.Amount==order.Total)
                _orderEventService.OrderFullyRefunded(order,orderRefund);
            else
                _orderEventService.OrderPartiallyRefunded(order, orderRefund);
        }

        public void Delete(OrderRefund item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}