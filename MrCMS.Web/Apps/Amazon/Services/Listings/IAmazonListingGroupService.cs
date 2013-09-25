using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public interface IAmazonListingGroupService
    {
        AmazonListingGroup Get(int id);
        IPagedList<AmazonListingGroup> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        AmazonListingGroup Save(AmazonListingGroup item);
        void Delete(AmazonListingGroup item);
        void InitAmazonListingsFromProductVariants(AmazonListingGroup amazonListingGroup, string rawProductVariantsIds);
        AmazonListing InitAmazonListingFromProductVariant(string productVariantSku, int amazonListingGroupId);
    }
}