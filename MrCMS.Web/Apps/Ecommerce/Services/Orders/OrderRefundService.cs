using System.Collections.Generic;
using MrCMS.Helpers;
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

        public void Save(OrderRefund item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderRefund item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}