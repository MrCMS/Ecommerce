using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IExportProductsManager _exportProductsManager;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IImportProductsManager _importExportManager;

        public ImportExportController(IImportProductsManager importExportManager, IExportProductsManager exportProductsManager, EcommerceSettings ecommerceSettings)
        {
            _importExportManager = importExportManager;
            _exportProductsManager = exportProductsManager;
            _ecommerceSettings = ecommerceSettings;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.View)]
        public ViewResult Products()
        {
            ViewData["warehousestockenabled"] = _ecommerceSettings.WarehouseStockEnabled;
            return View();
        }
        [HttpGet]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.CanExport)]
        public ActionResult ExportProducts()
        {
            try
            {
                var file = _exportProductsManager.ExportProductsToExcel();
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MrCMS-Products-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                TempData.ErrorMessages()
                    .Add(
                        "Products exporting has failed. Please try again and contact system administration if error continues to appear.");
                return RedirectToAction("Products");
            }
        }

        [HttpPost]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.CanImport)]
        public RedirectToRouteResult ImportProducts(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && document.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                Server.ScriptTimeout = 8000;
                var result = _importExportManager.ImportProductsFromExcel(document.InputStream);
                if (!result.Success)
                    TempData.ErrorMessages().AddRange(result.Errors);
                else
                    TempData.SuccessMessages().Add("The file was parsed and a batch has been generated.");
            }
            else
            {
                TempData.ErrorMessages().Add("Please choose non-empty Excel (.xslx) file before uploading.");
            }
            return RedirectToAction("Products", "ImportExport");
        }
    }
}