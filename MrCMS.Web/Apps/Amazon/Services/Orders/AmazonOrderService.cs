using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderService : IAmazonOrderService
    {
        private readonly ISession _session;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonOrderEventService _amazonOrderEventService;

        public AmazonOrderService(ISession session,
            IAmazonLogService amazonLogService, IAmazonOrderEventService amazonOrderEventService)
        {
            _session = session;
            _amazonLogService = amazonLogService;
            _amazonOrderEventService = amazonOrderEventService;
        }

        public AmazonOrder Get(int id)
        {
            return _session.Get<AmazonOrder>(id);
        }

        public AmazonOrder GetByOrderId(int id)
        {
            return _session.QueryOver<AmazonOrder>().Where(item => item.Order.Id == id).SingleOrDefault();
        }

        public AmazonOrder GetByAmazonOrderId(string id)
        {
            var orders=_session.QueryOver<AmazonOrder>().Where(item => item.AmazonOrderId == id).Cacheable().List();
            if (orders!=null && orders.Count > 1)
            {
                _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Error,
                       null, null, null, null, null, null, null, "Duplicate Amazon orders detected with #"+orders.First().AmazonOrderId);
                return orders.First();
            }
            if (orders != null && orders.Count == 1)
            {
                return orders.First();
            }
            return null;
        }

        public IPagedList<AmazonOrder> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonOrder>()
                                    .Where(x =>
                                        x.BuyerEmail.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.BuyerName.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.AmazonOrderId.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        ).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonOrder>(), page, pageSize);
        }

        public void Update(AmazonOrder item)
        {
            _session.Transact(session => session.Update(item));

            _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Update,
                                 null, null, null, null, item, null, null, "Amazon Order #" + item.AmazonOrderId);
        }

        public void SaveOrUpdate(AmazonOrder amazonOrder)
        {
            var orderId = amazonOrder.AmazonOrderId;
            var id = amazonOrder.Id;
            _session.Transact(session => session.SaveOrUpdate(amazonOrder));
            _amazonLogService.Add(AmazonLogType.Orders, id>0?AmazonLogStatus.Update:AmazonLogStatus.Insert, 
                null, null, null, null, amazonOrder, null, null,"Amazon Order #"+orderId);
        }

        public void Delete(AmazonOrder item)
        {
            _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Delete, 
                null, null, null, null, item, null, null, "Amazon Order #" + item.AmazonOrderId);

            _session.Transact(session => session.Delete(item));
        }

        public void SaveOrUpdate(List<AmazonOrder> orders)
        {
            _session.Transact(session => orders.ForEach(SaveOrUpdate));
        }

        public void MarkAsShipped(AmazonOrder amazonOrder)
        {
            amazonOrder.Status = AmazonOrderStatus.Shipped;
            SaveOrUpdate(amazonOrder);
        }
    }
}