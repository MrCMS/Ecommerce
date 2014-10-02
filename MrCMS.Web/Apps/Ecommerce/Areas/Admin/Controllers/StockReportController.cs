using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class StockReportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IStockReportAdminService _stockReportAdminService;

        public StockReportController(IStockReportAdminService stockReportAdminService)
        {
            _stockReportAdminService = stockReportAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof (StockReportACL), StockReportACL.CanExport)]
        public ActionResult Export()
        {
            var result = _stockReportAdminService.ExportStockReport();
            if (result.Success)
            {
                return result.FileResult;
            }
            TempData.ErrorMessages().Add(result.Message);
            return RedirectToAction("Index", "BulkStockUpdate");
        }
    }
}