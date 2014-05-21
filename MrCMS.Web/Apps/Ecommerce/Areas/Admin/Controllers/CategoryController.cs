using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CategoryController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICategoryAdminService _categoryAdminService;

        public CategoryController(ICategoryAdminService categoryAdminService)
        {
            _categoryAdminService = categoryAdminService;
        }

        public ViewResult Index(string q = null, int p = 1)
        {
            if (!_categoryAdminService.CategoryContainerExists())
                return View();
            IPagedList<Category> categoryPagedList = _categoryAdminService.Search(q, p);
            return View(categoryPagedList);
        }

        public JsonResult Search(string term, List<int> ids)
        {
            return Json(_categoryAdminService.Search(term, ids ?? new List<int>()));
        }
        public PartialViewResult HiddenSpecifications(Category category)
        {
            return PartialView(category);
        }

        [HttpGet]
        public JsonResult SearchCategories(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
                return
                    Json(
                        _categoryAdminService.Search(term)
                            .Select(x => new { x.Name, CategoryID = x.Id })
                            .Take(15)
                            .ToList());

            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult AddSpecification(Category category)
        {
            ViewData["category"] = category;
            return PartialView(_categoryAdminService.GetShownSpecifications(category));
        }

        [HttpPost]
        public JsonResult AddSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_categoryAdminService.AddSpecificationToHidden(attribute,categoryId));
        }

        [HttpPost]
        public JsonResult RemoveSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_categoryAdminService.RemoveSpecificationFromHidden(attribute,categoryId));
        }
    }
}