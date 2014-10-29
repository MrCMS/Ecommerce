using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class LowStockReportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ILowStockReportAdminService _lowStockReportAdminService;
        private readonly IUpdateStockAdminService _updateStockAdminService;

        public LowStockReportController(ILowStockReportAdminService lowStockReportAdminService,
            IUpdateStockAdminService updateStockAdminService)
        {
            _lowStockReportAdminService = lowStockReportAdminService;
            _updateStockAdminService = updateStockAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(LowStockReportACL), LowStockReportACL.View)]
        public ViewResult Index(LowStockReportSearchModel lowStockReportSearchModel)
        {
            ViewData["results"] = _lowStockReportAdminService.Search(lowStockReportSearchModel);
            ViewData["updated"] = TempData["updated"];
            return View(lowStockReportSearchModel);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(LowStockReportACL), LowStockReportACL.Update)]
        public JsonResult UpdateStock(int id, int stockRemaining)
        {
            _updateStockAdminService.UpdateVariantStock(id, stockRemaining);
            TempData["updated"] = id;
            return Json(true);
        }

        [MrCMSACLRule(typeof(LowStockReportACL), LowStockReportACL.CanExport)]
        public ActionResult Export(LowStockReportSearchModel searchModel)
        {
            var result = _lowStockReportAdminService.ExportLowStockReport(searchModel);
            if (result.Success)
            {
                return result.FileResult;
            }
            TempData.ErrorMessages().Add(result.Message);
            return RedirectToAction("Index");
        }
    }
}