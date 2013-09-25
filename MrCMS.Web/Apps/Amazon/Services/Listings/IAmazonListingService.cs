using MrCMS.Web.Apps.Amazon.Entities.Listings;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public interface IAmazonListingService
    {
        AmazonListing Get(int id);
        AmazonListing GetByProductVariantSKU(string sku);
        void Save(AmazonListing item);
        void Delete(AmazonListing item);
    }
}