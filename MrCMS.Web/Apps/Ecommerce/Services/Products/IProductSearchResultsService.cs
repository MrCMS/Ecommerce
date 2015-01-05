using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductSearchResultsService
    {
        void SetViewData(ProductSearchQuery productSearchQuery, ViewDataDictionary viewData);
    }
}