using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Filters
{
    public class SetProductSearchViewDataAttribute : ActionFilterAttribute
    {
        private IProductSearchQueryService _productSearchQueryService;

        public IProductSearchQueryService ProductSearchQueryService
        {
            get { return _productSearchQueryService ?? MrCMSApplication.Get<IProductSearchQueryService>(); }
            set { _productSearchQueryService = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var productSearchQuery =
                filterContext.ActionParameters.Values.FirstOrDefault(x => x.GetType() == typeof (ProductSearchQuery)) as ProductSearchQuery;

            if (productSearchQuery == null)
                return;

            ProductSearchQueryService.SetProductSearchViewData(productSearchQuery, filterContext.Controller.ViewData);
        }
    }
}