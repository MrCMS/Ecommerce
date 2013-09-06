using System.Web.Helpers;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        #region Props
        private readonly IImportExportManager _importExportManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IOptionService _optionService;
        private readonly IGoogleBaseService _googleBaseService;
        private readonly IProductVariantService _productVariantService;
        #endregion

        #region Ctor
        public ImportExportController(IImportExportManager importExportManager,
            IConfigurationProvider configurationProvider,
            GoogleBaseSettings googleBaseSettings,
            IOptionService optionService,
            IGoogleBaseService googleBaseService,
            IProductVariantService productVariantService)
        {
            _importExportManager = importExportManager;
            _configurationProvider = configurationProvider;
            _googleBaseSettings = googleBaseSettings;
            _optionService = optionService;
            _productVariantService = productVariantService;
            _googleBaseService = googleBaseService;
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
                var file = _importExportManager.ExportProductsToExcel();
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
        public JsonResult GetGoogleCategories(string term, int page=1)
        {
            var results = _googleBaseService.Search(term, page);

            return Json(new { Total = results.TotalItemCount, Items = results});
        }

        [HttpGet]
        public ViewResult GoogleBase(GoogleBaseModel model)
        {
            ViewData["settings"] = _googleBaseSettings;
            ViewData["categories"] = _optionService.GetCategoryOptions();
            ViewData["product-conditions"] = _optionService.GetEnumOptions<ProductCondition>();
            ViewData["genders"] = _optionService.GetEnumOptions<Gender>();
            ViewData["age-groups"] = _optionService.GetEnumOptions<AgeGroup>();

            model.Items = _productVariantService.GetAllVariants(model.Name, model.Category.HasValue ? model.Category.Value : 0, model.Page);

            return View(model);
        }
        [HttpGet]
        public ActionResult ExportProductsToGoogleBase()
        {
            try
            {
                var file = _importExportManager.ExportProductsToGoogleBase();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/rss+xml", "MrCMS-GoogleBase-Products.xml");
            }
            catch (Exception)
            {
                const string msg = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("GoogleBase", new { status = msg });
            }
        }

        [HttpPost]
        public ActionResult GoogleBaseSettings(GoogleBaseSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("GoogleBase");
        }

        [HttpPost]
        public JsonResult UpdateGoogleBaseProduct(GoogleBaseProduct googleBaseProduct)
        {
            try
            {
                _googleBaseService.SaveGoogleBaseProduct(googleBaseProduct);
                return Json(googleBaseProduct.Id);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return Json(false);
            }
        }

        [HttpGet]
        public ActionResult GoogleBaseProduct(GoogleBaseProduct googleBaseProduct)
        {
            if (googleBaseProduct != null)
            {
                ViewData["product-conditions"] = _optionService.GetEnumOptions<ProductCondition>();
                ViewData["genders"] = _optionService.GetEnumOptions<Gender>();
                ViewData["age-groups"] = _optionService.GetEnumOptions<AgeGroup>();
                return PartialView(googleBaseProduct);
            }
            return RedirectToAction("GoogleBase");
        }
        #endregion

        #region Orders

        [HttpGet]
        public ActionResult ExportOrderToPdf(Order order)
        {
            try
            {
                var file = _importExportManager.ExportOrderToPdf(order);
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