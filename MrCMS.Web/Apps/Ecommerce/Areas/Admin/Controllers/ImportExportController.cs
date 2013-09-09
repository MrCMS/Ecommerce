using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        #region Props
        private readonly IExportProductsManager _exportProductsManager;
        private readonly IImportProductsManager _importExportManager;
        private readonly IExportOrdersService _exportOrdersService;
        #endregion

        #region Ctor
        public ImportExportController(IImportProductsManager importExportManager, 
            IExportOrdersService exportOrdersService, IExportProductsManager exportProductsManager)
        {
            _importExportManager = importExportManager;
            _exportOrdersService = exportOrdersService;
            _exportProductsManager = exportProductsManager;
        }

        #endregion

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
                var file = _exportProductsManager.ExportProductsToExcel();
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

        #region Orders

        [HttpGet]
        public ActionResult ExportOrderToPdf(Order order)
        {
            try
            {
                var file = _exportOrdersService.ExportOrderToPdf(order);
                return File(file, "application/pdf", "MrCMS-Order-" + order.Id + "-["+CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm")+"].pdf");
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }
        }
        #endregion
    }
}