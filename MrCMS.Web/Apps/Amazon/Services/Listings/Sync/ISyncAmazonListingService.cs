using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface ISyncAmazonListingsService
    {
        void SyncAmazonListing(AmazonSyncModel model);
        void SyncAmazonListings(AmazonSyncModel model);

        void CloseAmazonListing(AmazonSyncModel model);
        void CloseAmazonListings(AmazonSyncModel model);
    }
}