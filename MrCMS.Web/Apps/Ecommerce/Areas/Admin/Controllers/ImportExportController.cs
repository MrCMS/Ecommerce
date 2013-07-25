using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Website.Controllers;
using System.Web;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IImportExportManager _importExportManager;

        public ImportExportController(IImportExportManager importExportManager)
        {
            _importExportManager = importExportManager;
        }

        #region Products
        [HttpGet]
        public ViewResult Products()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ExportProducts()
        {
            try
            {
                byte[] file = _importExportManager.ExportProductsToExcel();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MrCMS-Products-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception)
            {
                ViewBag.ExportStatus = "Products exporting has failed. Please try again and contact system administration if error continues to appear.";
                return View("Products");
            }
        }

        [HttpPost]
        public ViewResult ImportProducts(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && document.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                ViewBag.Messages = _importExportManager.ImportProductsFromExcel(document.InputStream);
            }
            else
            {
                ViewBag.ImportStatus = "Please choose non-empty Excel (.xslx) file before uploading.";
            }
            return View("Products");
        }
        #endregion

        #region Google Base
        [HttpGet]
        public ViewResult GoogleBase()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ExportProductsToGoogleBase()
        {
            try
            {
                byte[] file = _importExportManager.ExportProductsToGoogleBase();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/atom+xml", "MrCMS-GoogleBase-Products-" + DateTime.UtcNow + ".xml");
            }
            catch (Exception)
            {
                ViewBag.ExportStatus = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
                return View("GoogleBase");
            }
        }
        #endregion
    }
}