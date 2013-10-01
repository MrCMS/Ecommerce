using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface ISyncAmazonListingService
    {
        void SubmitProductFeeds(AmazonSyncModel model, AmazonListingGroup item);
        void SubmitSingleProductFeed(AmazonSyncModel model, AmazonListing item);
    }
}