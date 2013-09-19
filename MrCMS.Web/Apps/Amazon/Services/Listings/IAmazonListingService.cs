using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public interface IAmazonListingService
    {
        AmazonListing Get(int id);
        AmazonListing GetByAmazonListingId(string id);
        AmazonListing GetByProductVariantId(int id);
        IEnumerable<AmazonListing> GetAll();
        IPagedList<AmazonListing> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        void Save(AmazonListing item);
        void Delete(AmazonListing item);
        AmazonListing InitAmazonListingFromProductVariant(ProductVariant productVariant);
    }
}