using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
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

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAppAdminController<EcommerceApp>
    {
        #region Props
        private readonly IImportExportManager _importExportManager; 
        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IProductConditionService _productConditionService;
        private readonly ICategoryService _categoryService;
        private readonly IGoogleBaseTaxonomyService _googleBaseTaxonomyService;
        private readonly IProductVariantService _productVariantService;
        private readonly IGenderService _genderService;
        private readonly IAgeGroupService _ageGroupService;
        private readonly IGoogleBaseProductService _googleBaseProductService;
        #endregion

        #region Ctor
        public ImportExportController(IImportExportManager importExportManager,
            IConfigurationProvider configurationProvider, 
            GoogleBaseSettings googleBaseSettings,
            IProductConditionService productConditionService,
            ICategoryService categoryService,
            IGoogleBaseTaxonomyService googleBaseTaxonomyService,
            IProductVariantService productVariantService,
            IGenderService genderService,
            IAgeGroupService ageGroupService,
            IGoogleBaseProductService googleBaseProductService)
        {
            _importExportManager = importExportManager;
            _configurationProvider = configurationProvider;
            _googleBaseSettings = googleBaseSettings;
            _productConditionService = productConditionService;
            _categoryService = categoryService;
            _googleBaseTaxonomyService = googleBaseTaxonomyService;
            _productVariantService = productVariantService;
            _genderService = genderService;
            _ageGroupService = ageGroupService;
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
            ViewData["google-base-categories"] = _googleBaseTaxonomyService.GetOptions();
            ViewData["product-conditions"] = _productConditionService.GetOptions();
            ViewData["categories"] = _categoryService.GetOptions();
            ViewData["gender"] = _genderService.GetOptions();
            ViewData["age-group"] = _ageGroupService.GetOptions();

            model.Items = _productVariantService.GetAllVariants(model.Name, model.Category.HasValue ? model.Category.Value : 0, model.Page);
            return View(model);
        }
        [HttpGet]
        public ActionResult ExportProductsToGoogleBase()
        {
            //try
            //{
                var file = _importExportManager.ExportProductsToGoogleBase();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/rss+xml", "MrCMS-GoogleBase-Products.xml");
            //}
            //catch (Exception)
            //{
            //    const string msg = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
            //    return RedirectToAction("GoogleBase", new { status = msg });
            //}
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