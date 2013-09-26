using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface IAmazonListingSyncManager
    {
        void SyncAmazonListing(AmazonSyncModel model);
        void SyncAmazonListings(AmazonSyncModel model);

        void CloseAmazonListing(AmazonSyncModel model);
        void CloseAmazonListings(AmazonSyncModel model);

        AmazonSyncModel GetAmazonSyncModel(AmazonListing amazonListing);
        AmazonSyncModel GetAmazonSyncModel(AmazonListingGroup amazonListingGroup);
    }
}