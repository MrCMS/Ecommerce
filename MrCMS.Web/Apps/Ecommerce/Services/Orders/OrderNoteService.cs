using System.Collections.Generic;
using MrCMS.Helpers;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderNoteService : IOrderNoteService
    {
        private readonly ISession _session;

        public OrderNoteService(ISession session)
        {
            _session = session;
        }

        public IList<OrderNote> GetAll()
        {
            return _session.QueryOver<OrderNote>().Cacheable().List();
        }

        public void Save(OrderNote item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderNote item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}