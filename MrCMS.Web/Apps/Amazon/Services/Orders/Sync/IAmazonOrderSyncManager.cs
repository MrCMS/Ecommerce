using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IAmazonOrderSyncManager
    {
        void SyncOrders(AmazonSyncModel model);
    }
}