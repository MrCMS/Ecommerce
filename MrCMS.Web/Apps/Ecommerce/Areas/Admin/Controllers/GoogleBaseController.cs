using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class GoogleBaseController : MrCMSAppAdminController<EcommerceApp>
    {
        #region Props

        private readonly IConfigurationProvider _configurationProvider;
        private readonly GoogleBaseSettings _googleBaseSettings;
        private readonly IOptionService _optionService;
        private readonly IProductVariantService _productVariantService;
        private readonly IGoogleBaseManager _googleBaseManager;
        #endregion

        #region Ctor
        public GoogleBaseController(IConfigurationProvider configurationProvider,
            GoogleBaseSettings googleBaseSettings,
            IOptionService optionService,
            IProductVariantService productVariantService,
            IGoogleBaseManager googleBaseManager)
        {
            _configurationProvider = configurationProvider;
            _googleBaseSettings = googleBaseSettings;
            _optionService = optionService;
            _productVariantService = productVariantService;
            _googleBaseManager = googleBaseManager;
        }
        #endregion

        [HttpGet]
        public JsonResult GetGoogleCategories(string term, int page=1)
        {
            var results = _googleBaseManager.SearchGoogleBaseCategories(term, page);

            return Json(new { Total = results.TotalItemCount, Items = results});
        }

        [HttpGet]
        public ViewResult Dashboard(GoogleBaseModel model)
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
                var file = _googleBaseManager.ExportProductsToGoogleBase();
                ViewBag.ExportStatus = "Products successfully exported.";
                return File(file, "application/rss+xml", "MrCMS-GoogleBase-Products.xml");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                const string msg = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("Dashboard", new { status = msg });
            }
        }

        [HttpPost]
        public ActionResult GoogleBaseSettings(GoogleBaseSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public JsonResult UpdateGoogleBaseProduct(GoogleBaseProduct googleBaseProduct)
        {
            try
            {
                _googleBaseManager.SaveGoogleBaseProduct(googleBaseProduct);
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
            return RedirectToAction("Dashboard");
        }
    }
}