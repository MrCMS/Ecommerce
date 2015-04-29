using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductSearchQueryService
    {
        void SetProductSearchViewData(ProductSearchQuery query, ViewDataDictionary viewData);
        void SetBrandSearchViewData(ProductSearchQuery query, ViewDataDictionary viewData);
    }
}