using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public interface IAmazonListingGroupService
    {
        AmazonListingGroup Get(int id);
        IPagedList<AmazonListingGroup> Search(string queryTerm = null, int page = 1, int pageSize = 10);
        void Save(AmazonListingGroup item);
        void Delete(AmazonListingGroup item);
    }
}