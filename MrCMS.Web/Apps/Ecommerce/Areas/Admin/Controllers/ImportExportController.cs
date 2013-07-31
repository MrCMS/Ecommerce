using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
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
using MrCMS.Web.Apps.Ecommerce.Services.Users;
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
        private readonly ICategoryService _categoryService;
        private readonly IGoogleBaseService _googleBaseTaxonomyService;
        private readonly IProductVariantService _productVariantService;
        private readonly IGoogleBaseProductService _googleBaseProductService;
        #endregion

        #region Ctor
        public ImportExportController(IImportExportManager importExportManager,
            IConfigurationProvider configurationProvider, 
            GoogleBaseSettings googleBaseSettings,
            IOptionService optionService,
            ICategoryService categoryService,
            IGoogleBaseService googleBaseTaxonomyService,
            IProductVariantService productVariantService,
            IGoogleBaseProductService googleBaseProductService)
        {
            _importExportManager = importExportManager;
            _configurationProvider = configurationProvider;
            _googleBaseSettings = googleBaseSettings;
            _optionService = optionService;
            _categoryService = categoryService;
            _googleBaseTaxonomyService = googleBaseTaxonomyService;
            _productVariantService = productVariantService;
            _googleBaseProductService = googleBaseProductService;
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
        public ViewResult GoogleBase(GoogleBaseModel model)
        {
            ViewData["settings"] = _googleBaseSettings;
            ViewData["google-base-categories"] = _googleBaseTaxonomyService.GetGoogleCategories();
            ViewData["categories"] = _categoryService.GetOptions();
            ViewData["product-conditions"] = _optionService.GetEnumOptions<ProductCondition>();
            ViewData["gender"] = _optionService.GetEnumOptions<Gender>();
            ViewData["age-group"] = _optionService.GetEnumOptions<AgeGroup>();

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
        public JsonResult UpdateGoogleBaseRecord(ProductVariant productVariant,
                                                   string overrideCategory,
                                                   string grouping,
                                                   string labels,
                                                   string redirect,
                                                   int googleBaseProductId = 0,
                                                   ProductCondition overrideCondition = ProductCondition.New,
                                                   Gender gender = Gender.Male,
                                                   AgeGroup ageGroup = AgeGroup.Adults)
        {
            if (productVariant != null)
            {
                var googleBaseProduct = _googleBaseProductService.Get(googleBaseProductId)
                                        ?? new GoogleBaseProduct(){ProductVariant = productVariant};
                if (!String.IsNullOrWhiteSpace(overrideCategory))
                    googleBaseProduct.OverrideCategory = overrideCategory;
                if (!String.IsNullOrWhiteSpace(grouping))
                    googleBaseProduct.Grouping = grouping;
                if (!String.IsNullOrWhiteSpace(labels))
                    googleBaseProduct.Labels = labels;
                if (!String.IsNullOrWhiteSpace(redirect))
                    googleBaseProduct.Redirect = redirect;
                googleBaseProduct.OverrideCondition = overrideCondition;
                googleBaseProduct.Gender = gender;
                googleBaseProduct.AgeGroup = ageGroup;
                googleBaseProduct.Site = CurrentSite;

                if(googleBaseProductId==0)
                    _googleBaseProductService.Add(googleBaseProduct);
                else
                    _googleBaseProductService.Update(googleBaseProduct);
                productVariant.GoogleBaseProduct= googleBaseProduct;
                _productVariantService.Update(productVariant);

                return Json(true);
            }
            return Json(false);
        }

        #endregion
    }
}