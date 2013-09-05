using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using System.Linq;
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
            ViewBag.Categories = _categoryService.GetAll().Where(x => x.Parent != null && x.Parent.Parent == null).ToList();
            return View(page);
        }
    }
}