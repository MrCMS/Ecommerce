using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductSearchQueryService
    {
        void SetViewData(ProductSearchQuery query, ViewDataDictionary viewData);
    }
}