using System.Collections.Generic;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public class GetCustomerInteraction : IGetCustomerInteraction
    {
        private readonly ISession _session;

        public GetCustomerInteraction(ISession session)
        {
            _session = session;
        }

        public IList<CorrespondenceRecord> Get(Order order)
        {
            return
                _session.QueryOver<CorrespondenceRecord>()
                    .Where(x => x.Order.Id == order.Id)
                    .OrderBy(x => x.CreatedOn)
                    .Desc.Cacheable()
                    .List();
        }
    }
}