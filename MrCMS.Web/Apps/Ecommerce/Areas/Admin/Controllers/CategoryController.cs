using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Website.Controllers;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CategoryController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _siteSettings;
        private readonly IUniquePageService _uniquePageService;

        public CategoryController(ICategoryService categoryService, IDocumentService documentService, SiteSettings siteSettings, IUniquePageService uniquePageService)
        {
            _categoryService = categoryService;
            _documentService = documentService;
            _siteSettings = siteSettings;
            _uniquePageService = uniquePageService;
        }

        public ViewResult Index(string q = null, int p = 1)
        {
            if (_uniquePageService.GetUniquePage<CategoryContainer>() == null)
                return View();
            var categoryPagedList = _categoryService.Search(q, p, _siteSettings.DefaultPageSize);
            return View(categoryPagedList);
        }

        public JsonResult Search(string term, List<int> ids)
        {
            return Json(_categoryService.Search(term, ids ?? new List<int>()));
        }

        [HttpGet]
        public JsonResult SearchCategories(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
                return Json(_categoryService.Search(term).Select(x => new { Name = x.Name, CategoryID = x.Id }).Take(15).ToList());

            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}