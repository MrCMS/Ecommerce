using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSyncDataService
    {
        AmazonOrderSyncData Get(int id);
        IList<AmazonOrderSyncData> GetAllByOperationType(SyncAmazonOrderOperation operation, int pagesize = 25);
        AmazonOrderSyncData Add(AmazonOrderSyncData item);
        AmazonOrderSyncData Update(AmazonOrderSyncData item);
        void Delete(AmazonOrderSyncData item);
        void MarkAllAsPendingIfNotSyncedAfterOneHour();
    }
}