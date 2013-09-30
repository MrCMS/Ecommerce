using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface ICloseAmazonListingService
    {
        void CloseAmazonListing(AmazonSyncModel syncModel, AmazonListing amazonListing);
        void CloseAmazonListings(AmazonSyncModel syncModel, AmazonListingGroup amazonListingGroup);
    }
}