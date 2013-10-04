using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryContainerController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;

        public CategoryContainerController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ViewResult Show(CategoryContainer page)
        {
            return View(page);
        }

        [HttpGet]
        public PartialViewResult Categories(int page = 1)
        {
            return PartialView(_categoryService.GetRootCategories());
        }
    }
}