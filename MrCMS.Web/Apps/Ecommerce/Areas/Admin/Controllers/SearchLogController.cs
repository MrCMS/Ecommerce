using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Search;
using MrCMS.Web.Apps.Ecommerce.Services.Search;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class SearchLogController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ISearchLogService _searchLogService;
        private readonly SiteSettings _siteSettings;

        public SearchLogController(ISearchLogService searchLogService, SiteSettings siteSettings)
        {
            _searchLogService = searchLogService;
            _siteSettings = siteSettings;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SearchLogACL), SearchLogACL.List)]
        public ActionResult Index(string q, int page = 1)
        {
            ViewData["query"] = q;
            var searchLogs = _searchLogService.GetPaged(page, q, _siteSettings.DefaultPageSize);
            return View(searchLogs);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SearchLogACL), SearchLogACL.Edit)]
        public ViewResult Edit(SearchLog searchLog)
        {
            return View(searchLog);
        }

        [ActionName("Edit")]
        [HttpPost]
        [MrCMSACLRule(typeof(SearchLogACL), SearchLogACL.Edit)]
        public ActionResult Edit_POST(SearchLog searchLog)
        {
            if (ModelState.IsValid)
            {
                _searchLogService.Update(searchLog);
                return RedirectToAction("Index");
            }
            return View(searchLog);
        }

        [HttpGet]
        public PartialViewResult Delete(SearchLog searchLog)
        {
            return PartialView(searchLog);
        }

        [ActionName("Delete")]
        [HttpPost]
        [MrCMSACLRule(typeof(SearchLogACL), SearchLogACL.Delete)]
        public RedirectToRouteResult Delete_POST(SearchLog searchLog)
        {
            _searchLogService.Delete(searchLog);
            return RedirectToAction("Index");
        }
    }
}