using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;
using System.Web;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IImportExportManager _importExportManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IProductConditionService _productConditionService;
        private readonly ICategoryService _categoryService;
        private readonly IGoogleBaseTaxonomyService _googleBaseTaxonomyService;
        private readonly IProductVariantService _productVariantService;

        public ImportExportController(IImportExportManager importExportManager,
            IConfigurationProvider configurationProvider, 
            GoogleBaseSettings googleBaseSettings,
            IProductConditionService productConditionService,
            ICategoryService categoryService,
            IGoogleBaseTaxonomyService googleBaseTaxonomyService,
            IProductVariantService productVariantService)
        {
            _importExportManager = importExportManager;
            _configurationProvider = configurationProvider;
            _googleBaseSettings = googleBaseSettings;
            _productConditionService = productConditionService;
            _categoryService = categoryService;
            _googleBaseTaxonomyService = googleBaseTaxonomyService;
            _productVariantService = productVariantService;
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
        public ViewResult GoogleBase(GoogleBaseModel model)
        {
            ViewData["settings"] = _googleBaseSettings;
            ViewData["google-base-categories"] = _googleBaseTaxonomyService.GetOptions();
            ViewData["product-conditions"] = _productConditionService.GetOptions();
            ViewData["categories"] = _categoryService.GetOptions();

            model.Items = _productVariantService.GetAllVariants(model.Name, model.Category.HasValue ? model.Category.Value : 0, model.Page);
            return View(model);
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
                var msg = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("GoogleBase", new { status = msg });
            }
        }
        [HttpPost]
        public ActionResult GoogleBaseSettings(GoogleBaseSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("GoogleBase");
        }
        #endregion
    }
}