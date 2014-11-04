using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderRefundService : IOrderRefundService
    {
        private readonly ISession _session;

        public OrderRefundService(ISession session)
        {
            _session = session;
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
                EventContext.Instance.Publish<IOnOrderFullyRefunded, OrderFullyRefundedArgs>
                    (new OrderFullyRefundedArgs {Refund = orderRefund, Order = orderRefund.Order});
            else
                EventContext.Instance.Publish<IOnOrderPartiallyRefunded, OrderPartiallyRefundedArgs>
                    (new OrderPartiallyRefundedArgs { Refund = orderRefund, Order = orderRefund.Order });
        }

        public void Delete(OrderRefund item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}