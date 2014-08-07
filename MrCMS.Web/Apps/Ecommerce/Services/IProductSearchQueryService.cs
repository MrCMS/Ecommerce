using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductSearchQueryService
    {
        void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData);
        CachingInfo GetCachingInfo(ProductSearchQuery query);
    }
}