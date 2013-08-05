using System;
using System.Web.Mvc;
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
        public ViewResult LowStockReport(string status,int t=10, int page = 1)
        {
            if(!String.IsNullOrWhiteSpace(status))
                ViewBag.Status = status;
            ViewData["treshold"] = t;
            var items = _productVariantService.GetAllVariantsWithLowStock(t, page);
            return View(items);
        }

        [HttpPost]
        public RedirectToRouteResult UpdateStock(ProductVariant productVariant, int t=10)
        {
            if (productVariant != null && productVariant.StockRemaining.HasValue)
            {
                var pv = _productVariantService.Get(productVariant.Id);
                pv.StockRemaining = productVariant.StockRemaining.Value;
                _productVariantService.Update(productVariant);
            }
            return RedirectToAction("LowStockReport",new {t=t});
        }

        [HttpPost]
        public ActionResult ExportLowStockReport(int treshold = 10)
        {
            try
            {
                var file = _inventoryService.ExportLowStockReport(treshold);
                ViewBag.ExportStatus = "Low Stock Report successfully exported.";
                return File(file, "text/csv", "MrCMS-LowStockReport-"+DateTime.UtcNow+".csv");
            }
            catch (Exception)
            {
                const string msg = "Low Stock Report exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("LowStockReport", new {status=msg});
            }
        }

        [HttpGet]
        public ViewResult BulkStockUpdate()
        {
            return View();
        }
    }
}