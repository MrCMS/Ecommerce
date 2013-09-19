using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface ISyncAmazonListingsService
    {
        void ExportAmazonListing(AmazonSyncModel model);
    }
}