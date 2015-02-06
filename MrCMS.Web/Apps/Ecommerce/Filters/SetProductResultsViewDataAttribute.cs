using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Filters
{
    public class SetProductResultsViewDataAttribute : ActionFilterAttribute
    {
        private IProductSearchResultsService _productSearchQueryService;

        public IProductSearchResultsService ProductSearchQueryService
        {
            get { return _productSearchQueryService ?? MrCMSApplication.Get<IProductSearchResultsService>(); }
            set { _productSearchQueryService = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var productSearchQuery =
                filterContext.ActionParameters.Values.FirstOrDefault(x => x.GetType() == typeof (ProductSearchQuery)) as ProductSearchQuery;

            if (productSearchQuery == null)
                return;

            ProductSearchQueryService.SetViewData(productSearchQuery, filterContext.Controller.ViewData);
        }
    }
}