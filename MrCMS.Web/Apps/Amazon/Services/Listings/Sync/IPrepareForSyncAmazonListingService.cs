using MrCMS.Web.Apps.Amazon.Entities.Listings;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public interface IPrepareForSyncAmazonListingService
    {
        void UpdateAmazonListings(AmazonListingGroup amazonListingGroup);
        AmazonListing UpdateAmazonListing(AmazonListing amazonListing);

        void InitAmazonListingsFromProductVariants(AmazonListingGroup amazonListingGroup, string rawProductVariantsIds);
        AmazonListing InitAmazonListingFromProductVariant(AmazonListing amazonListing, string productVariantSku,
                                                          int amazonListingGroupId);
    }
}