using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductSearchQueryExtensions
    {
        public static CachingInfo GetCachingInfo(this ProductSearchQuery query)
        {
            return MrCMSApplication.Get<IProductSearchIndexService>().GetCachingInfo(query);
        }
    }
}