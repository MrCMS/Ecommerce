using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class StockController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IInventoryService _inventoryService;

        public StockController(IProductVariantService productVariantService, IInventoryService inventoryService)
        {
            _productVariantService = productVariantService;
            _inventoryService = inventoryService; 
        }

        [HttpGet]
        public ViewResult LowStockReport(int threshold = 10)
        {
            if (TempData.ContainsKey("export-status"))
                ViewBag.ExportStatus = TempData["export-status"];
            ViewData["threshold"] = threshold;
            return View();
        }

        [HttpGet]
        public PartialViewResult LowStockReportProductVariants(int threshold = 10, int page = 1)
        {
            var items = _productVariantService.GetAllVariantsWithLowStock(threshold, page);
            return PartialView(items);
        }

        [HttpPost]
        public JsonResult UpdateStock(ProductVariant productVariant, int threshold=10)
        {
            if (productVariant != null && productVariant.StockRemaining.HasValue)
            {
                var pv = _productVariantService.Get(productVariant.Id);
                pv.StockRemaining = productVariant.StockRemaining.Value;
                _productVariantService.Update(productVariant);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult ExportLowStockReport(int threshold = 10)
        {
            try
            {
                var file = _inventoryService.ExportLowStockReport(threshold);
                ViewBag.ExportStatus = "Low Stock Report successfully exported.";
                return File(file, "text/csv", "MrCMS-LowStockReport-"+DateTime.UtcNow+".csv");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                ViewBag.ExportStatus = "Low Stock Report exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("LowStockReport");
            }
        }

        [HttpGet]
        public ViewResult BulkStockUpdate()
        {
            if (TempData.ContainsKey("messages"))
                ViewBag.Messages = TempData["messages"];
            if (TempData.ContainsKey("import-status"))
                ViewBag.ImportStatus = TempData["import-status"];
            if (TempData.ContainsKey("export-status"))
                ViewBag.ExportStatus = TempData["export-status"];
            return View();
        }

        [HttpPost]
        [ActionName("BulkStockUpdate")]
        public RedirectToRouteResult BulkStockUpdate_POST(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && (document.ContentType.ToLower() == "text/csv" || document.ContentType.ToLower().Contains("excel")))
                TempData["messages"] = _inventoryService.BulkStockUpdate(document.InputStream);
            else
                TempData["import-status"] = "Please choose non-empty CSV (.csv) file before uploading.";
            return RedirectToAction("BulkStockUpdate");
        }

        [HttpGet]
        public ActionResult ExportStockReport()
        {
            try
            {
                var file = _inventoryService.ExportStockReport();
                TempData["export-status"] = "Stock Report successfully exported.";
                return File(file, "text/csv", "MrCMS-StockReport-" + DateTime.UtcNow + ".csv");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                TempData["export-status"] = "Stock Report exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("BulkStockUpdate");
            }
        }
    }
}