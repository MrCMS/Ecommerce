using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public interface IAmazonListingService
    {
        AmazonListing Get(int id);
        AmazonListing GetByProductVariantSku(string sku);
        void Save(AmazonListing item);
        void Delete(AmazonListing item);

        void UpdateAmazonListingStatus(AmazonListing item);
        void UpdateAmazonListingStatusAndAsin(AmazonListing item, Product amazonProduct);
        AmazonListingModel GetAmazonListingModel(AmazonListingGroup amazonListingGroup);
        AmazonListingModel GetAmazonListingModel(AmazonListingModel oldModel);
    }
}