using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderSyncDataService : IAmazonOrderSyncDataService
    {
        private readonly ISession _session;
        private readonly IAmazonLogService  _amazonLogService;

        public AmazonOrderSyncDataService(ISession session, IAmazonLogService amazonLogService)
        {
            _session = session;
            _amazonLogService = amazonLogService;
        }

        public AmazonOrderSyncData Get(int id)
        {
            return _session.Get<AmazonOrderSyncData>(id);
        }

        public AmazonOrderSyncData GetByAmazonOrderId(string id)
        {
            return _session.QueryOver<AmazonOrderSyncData>().Where(item => item.OrderId == id).SingleOrDefault();
        }

        public IList<AmazonOrderSyncData> GetAllByOperationType(SyncAmazonOrderOperation operation, int pagesize = 25)
        {
            return _session.QueryOver<AmazonOrderSyncData>()
                            .Where(item => item.Operation == operation && item.Status == SyncAmazonOrderStatus.Pending)
                            .OrderBy(x => x.CreatedOn).Asc.Take(pagesize).Cacheable().List();
        }

        public void MarkAllAsPendingIfNotSyncedAfterOneHour()
        {
            var items = _session.QueryOver<AmazonOrderSyncData>()
                                .Where(item => item.Status == SyncAmazonOrderStatus.InProgress).Cacheable().List();

            foreach (var amazonOrderSyncData in items.Where(x => (CurrentRequestData.Now - x.CreatedOn).Hours >= 1))
            {
                amazonOrderSyncData.Status = SyncAmazonOrderStatus.Pending;
                Update(amazonOrderSyncData);
            }
        }

        public AmazonOrderSyncData Add(AmazonOrderSyncData item)
        {
            _session.Transact(session =>
                {
                    if (item.AmazonOrder != null)
                    {
                        item.AmazonOrder = session.Get<AmazonOrder>(item.AmazonOrder.Id);
                        item.Site = session.Get<Site>(item.AmazonOrder.Site.Id);
                    }
                    session.Save(item);
                });
            _amazonLogService.Add(AmazonLogType.OrdersSyncData, AmazonLogStatus.Insert,
                                null, null, null, null, null, null, null, "Amazon Order #" + item.OrderId);
            return item;
        }

        public AmazonOrderSyncData Update(AmazonOrderSyncData item)
        {
            _session.Transact(session =>
            {
                if (item.AmazonOrder != null)
                {
                    item.AmazonOrder = session.Get<AmazonOrder>(item.AmazonOrder.Id);
                    item.Site = session.Get<Site>(item.AmazonOrder.Site.Id);
                }
                session.Update(item);
            });
            _amazonLogService.Add(AmazonLogType.OrdersSyncData, AmazonLogStatus.Update,
                                null, null, null, null, null, null, null,"Amazon Order #"+item.OrderId);
            return item;
        }

        public void Delete(AmazonOrderSyncData item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}