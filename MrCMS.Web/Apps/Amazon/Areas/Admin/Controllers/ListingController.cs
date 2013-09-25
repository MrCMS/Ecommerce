using System;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
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
        private readonly IAmazonListingGroupService _amazonListingGroupService;

        public ListingController(
            IProductVariantService productVariantService, 
            ISyncAmazonListingsService syncAmazonListingsService, 
            IAmazonListingService amazonListingService, 
            IOptionService optionService, 
            AmazonAppSettings amazonAppSettings, 
            IAmazonListingGroupService amazonListingGroupService)
        {
            _productVariantService = productVariantService;
            _syncAmazonListingsService = syncAmazonListingsService;
            _amazonListingService = amazonListingService;
            _optionService = optionService;
            _amazonAppSettings = amazonAppSettings;
            _amazonListingGroupService = amazonListingGroupService;
        }


        [HttpGet]
        public ActionResult Details(AmazonListing amazonListing)
        {
            if (amazonListing != null)
                return View(amazonListing);
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpGet]
        public ViewResult ChooseProductVariant(AmazonListingGroup amazonListingGroup)
        {
            ViewData["categories"] = _optionService.GetCategoryOptions();
            var model = new AmazonListingModel()
                {
                    ProductVariants = _productVariantService.GetAllVariants(String.Empty),
                    AmazonListingGroup = amazonListingGroup
                };
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
        public ActionResult AddOne(string productVariantSku, int amazonListingGroupId)
        {
            if (!String.IsNullOrWhiteSpace(productVariantSku) && amazonListingGroupId > 0)
            {
                var amazonListing = _amazonListingService.GetByProductVariantSKU(productVariantSku);
                if (amazonListing == null)
                {
                    amazonListing = _amazonListingGroupService.InitAmazonListingFromProductVariant(productVariantSku, amazonListingGroupId);
                    return View(amazonListing);
                }

                return RedirectToAction("Details", new {id = amazonListing.Id});
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("AddOne")]
        public ActionResult AddOne_POST(AmazonListing listing)
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
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpGet]
        [ActionName("SyncOne")]
        public ActionResult SyncOne_GET(AmazonListing listing)
        {
            if (listing != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                return View(new AmazonSyncModel() { Id = listing.Id, Title = listing.Title });
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        public JsonResult SyncOne(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonListingsService.SyncAmazonListing(model);
                return Json(true);
            }
            return Json(false);
        }


        [HttpGet]
        public ActionResult AddMany(AmazonListingGroup amazonListingGroup)
        {
            return View(new AmazonListingModel() { AmazonListingGroup = amazonListingGroup });
        }

        [HttpPost]
        [ActionName("AddMany")]
        public ActionResult AddMany_POST(AmazonListingModel model)
        {
            if (model != null && model.AmazonListingGroup!=null && !String.IsNullOrWhiteSpace(model.ChosenProductVariants))
            {
                _amazonListingGroupService.InitAmazonListingsFromProductVariants(model.AmazonListingGroup, model.ChosenProductVariants);
                return RedirectToAction("SyncMany", new { id = model.AmazonListingGroup.Id });
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpGet]
        [ActionName("SyncMany")]
        public ActionResult SyncMany_GET(AmazonListingGroup amazonListingGroup)
        {
            if (amazonListingGroup != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                return View(new AmazonSyncModel() {Id=amazonListingGroup.Id,Title = amazonListingGroup.Name});
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        public JsonResult SyncMany(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonListingsService.SyncAmazonListings(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult CloseOne(AmazonListing listing)
        {
            if (listing != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                return View(new AmazonSyncModel() { Id = listing.Id, Title = listing.Title, Description = listing.ASIN });
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("CloseOne")]
        public JsonResult CloseOne_POST(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonListingsService.CloseAmazonListing(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult CloseMany(AmazonListingGroup amazonListingGroup)
        {
            if (amazonListingGroup != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                return View(new AmazonSyncModel() { Id = amazonListingGroup.Id, Title = amazonListingGroup.Name });
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("CloseMany")]
        public JsonResult CloseMany_POST(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonListingsService.CloseAmazonListings(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult Delete(AmazonListing amazonListing)
        {
            if (amazonListing != null)
                return View(amazonListing);
            return RedirectToAction("Index","ListingGroup");
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(AmazonListing amazonListing)
        {
            _amazonListingService.Delete(amazonListing);
           return RedirectToAction("Edit","ListingGroup",new {id=amazonListing.AmazonListingGroup.Id});
        }
    }
}
