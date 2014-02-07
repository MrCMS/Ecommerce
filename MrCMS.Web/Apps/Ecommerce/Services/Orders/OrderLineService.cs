using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderLineService : IOrderLineService 
    {
        private readonly ISession _session;
        private readonly IOrderNoteService _orderNoteService;

        public OrderLineService(ISession session, IOrderNoteService orderNoteService)
        {
            _session = session;
            _orderNoteService = orderNoteService;
        }

        public void Save(OrderLine item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public OrderLine Get(int id)
        {
            return _session.QueryOver<OrderLine>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}