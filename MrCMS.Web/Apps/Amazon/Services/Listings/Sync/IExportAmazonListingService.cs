using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface IExportAmazonListingService
    {
        void SubmitProductFeeds(AmazonSyncModel model, AmazonListing item);
    }
}