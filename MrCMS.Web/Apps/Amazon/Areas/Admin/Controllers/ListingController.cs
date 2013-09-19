using System;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class ListingController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ISyncAmazonListingsService _syncAmazonListingsService;
        private readonly IAmazonListingService _amazonListingService;
        private readonly IOptionService _optionService;
        private readonly AmazonAppSettings _amazonAppSettings;

        public ListingController(
            IProductVariantService productVariantService, 
            ISyncAmazonListingsService syncAmazonListingsService, 
            IAmazonListingService amazonListingService, 
            IOptionService optionService, 
            AmazonAppSettings amazonAppSettings)
        {
            _productVariantService = productVariantService;
            _syncAmazonListingsService = syncAmazonListingsService;
            _amazonListingService = amazonListingService;
            _optionService = optionService;
            _amazonAppSettings = amazonAppSettings;
        }

        [HttpGet]
        public ViewResult Index(string searchTerm,int page = 1)
        {
            ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
            var results = _amazonListingService.Search(searchTerm,page);
            return View(results);
        }

        [HttpGet]
        public PartialViewResult Listings(string searchTerm, int page = 1)
        {
            var results = _amazonListingService.Search(searchTerm, page);
            return PartialView(results);
        }

        [HttpGet]
        public ViewResult ChooseProductVariant()
        {
            ViewData["categories"] = _optionService.GetCategoryOptions();
            var model = new AmazonListingModel() { ProductVariants = _productVariantService.GetAllVariants(String.Empty) };
            return View(model);
        }

        [HttpGet]
        public ActionResult ProductVariants(AmazonListingModel model)
        {
            ViewData["categories"] = _optionService.GetCategoryOptions();
            var newModel = new AmazonListingModel() { ProductVariants = _productVariantService.GetAllVariants(model.Name, model.CategoryId, model.Page) };
            return PartialView(newModel);
        }

        [HttpGet]
        public ActionResult Add(ProductVariant productVariant)
        {
            if (productVariant != null && productVariant.Id > 0)
            {
                var amazonListing = _amazonListingService.GetByProductVariantId(productVariant.Id);
                if (amazonListing == null)
                {
                    amazonListing = _amazonListingService.InitAmazonListingFromProductVariant(productVariant);
                    return View(amazonListing);
                }
                return RedirectToAction("Details", new {id = amazonListing.Id});
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Add")]
        public ActionResult Add_POST(AmazonListing listing)
        {
            if (listing != null)
            {
                if (ModelState.IsValid)
                {
                    listing.Status = AmazonListingStatus.NotOnAmazon;
                    _amazonListingService.Save(listing);

                    return RedirectToAction("SyncOne", new { id = listing.Id });
                }
                return View(listing);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SyncOne(AmazonListing listing)
        {
            if (listing != null)
                return View(new AmazonSyncModel() { Id = listing.Id, Title = listing.Title });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Sync(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonListingsService.ExportAmazonListing(model);
                return Json(true);
            }
            return Json(false);
        }
    }
}
