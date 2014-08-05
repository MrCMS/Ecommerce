using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductSearchQueryService _productSearchQueryService;
        private readonly ICategoryService _categoryService;

        public CategoryController(IProductSearchQueryService productSearchQueryService, ICategoryService categoryService)
        {
            _productSearchQueryService = productSearchQueryService;
            _categoryService = categoryService;
        }

        public ViewResult Show(Category page,
            [IoCModelBinder(typeof (ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            query.CategoryId = page.Id;
            ViewData["query"] = query;
            _productSearchQueryService.SetViewData(query, ViewData);
            if (page.ShowSubCategories)
            {
                ViewData["subcategories"] = _categoryService.GetSubCategories(page);
            }
            return View(page);
        }
    }
}