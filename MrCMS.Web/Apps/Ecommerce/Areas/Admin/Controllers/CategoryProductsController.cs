using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CategoryProductsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICategoryProductsAdminService _categoryProductsAdminService;

        public CategoryProductsController(ICategoryProductsAdminService categoryProductsAdminService)
        {
            _categoryProductsAdminService = categoryProductsAdminService;
        }

        [HttpGet]
        public ViewResult Sort(Category category)
        {
            ViewData["category-products"] = _categoryProductsAdminService.GetProductSortData(category);
            ViewData["is-sorted"] = _categoryProductsAdminService.IsSorted(category);
            return View(category);
        }

        [HttpPost, ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Sort(Category category,
            [IoCModelBinder(typeof(ProductSortDataModelBinder))] List<ProductSortData> productSortData)
        {
            _categoryProductsAdminService.Update(category, productSortData);
            TempData.SuccessMessages().Add("Products sorted");
            return RedirectToAction("Sort", new { id = category.Id });
        }

        [HttpGet]
        public ViewResult Clear(Category category)
        {
            return View(category);
        }

        [HttpPost, ActionName("Clear"), ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Clear_POST(Category category)
        {
            _categoryProductsAdminService.ClearSorting(category);
            TempData.SuccessMessages().Add("Sorting cleared");
            return RedirectToAction("Sort", new { id = category.Id });
        }
    }
}