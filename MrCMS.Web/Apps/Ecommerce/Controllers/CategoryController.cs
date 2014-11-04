using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ViewResult Show(Category page,
            [IoCModelBinder(typeof(ProductSearchQueryModelBinder))] ProductSearchQuery query)
        {
            query.CategoryId = page.Id;
            ViewData["query"] = query;
            if (page.ShowSubCategories)
            {
                ViewData["subcategories"] = _categoryService.GetSubCategories(page);
            }
            return View(page);
        }
    }
}