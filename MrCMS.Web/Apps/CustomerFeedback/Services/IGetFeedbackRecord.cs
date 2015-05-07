using System;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public interface IGetFeedbackRecord
    {
        FeedbackRecord GetByOrderGuid(Guid guid);
    }

    public class GetFeedbackRecord : IGetFeedbackRecord
    {
        private readonly ISession _session;

        public GetFeedbackRecord(ISession session)
        {
            _session = session;
        }

        public FeedbackRecord GetByOrderGuid(Guid guid)
        {
            var order = _session.QueryOver<Order>().Where(o => o.Guid == guid).Take(1).Cacheable().SingleOrDefault();

            return
                _session.QueryOver<FeedbackRecord>()
                    .Where(o => o.Order.Id == order.Id)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault();
        }
    }
}