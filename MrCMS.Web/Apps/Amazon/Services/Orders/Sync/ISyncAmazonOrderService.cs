using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface ISyncAmazonOrderService
    {
        void SyncOrders(AmazonSyncModel model);
    }
}