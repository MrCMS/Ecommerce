using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductSearchQueryService _productSearchQueryService;

        public CategoryController(IProductSearchQueryService productSearchQueryService)
        {
            _productSearchQueryService = productSearchQueryService;
        }

        public ViewResult Show(Category page,
            [IoCModelBinder(typeof (ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            query.CategoryId = page.Id;
            ViewData["query"] = query;
            _productSearchQueryService.SetViewData(query, ViewData);
            return View(page);
        }
    }
}