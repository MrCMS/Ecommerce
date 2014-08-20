using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IExportProductsManager _exportProductsManager;
        private readonly IImportProductsManager _importExportManager;

        public ImportExportController(IImportProductsManager importExportManager, IExportProductsManager exportProductsManager)
        {
            _importExportManager = importExportManager;
            _exportProductsManager = exportProductsManager;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.View)]
        public ViewResult Products()
        {
            return View();
        }
        [HttpGet]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.CanExport)]
        public ActionResult ExportProducts()
        {
            try
            {
                var file = _exportProductsManager.ExportProductsToExcel();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MrCMS-Products-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                ViewBag.ExportStatus = "Products exporting has failed. Please try again and contact system administration if error continues to appear.";
                return View("Products");
            }
        }

        [HttpPost]
        [MrCMSACLRule(typeof(ImportExportACL), ImportExportACL.CanImport)]
        public ViewResult ImportProducts(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && document.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                Server.ScriptTimeout = 8000;
                ViewBag.Messages = _importExportManager.ImportProductsFromExcel(document.InputStream);
            }
            else
            {
                ViewBag.ImportStatus = "Please choose non-empty Excel (.xslx) file before uploading.";
            }
            return View("Products");
        }
    }
}