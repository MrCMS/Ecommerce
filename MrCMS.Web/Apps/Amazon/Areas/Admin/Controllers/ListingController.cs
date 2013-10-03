using System;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ListingController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonListingSyncManager _amazonListingSyncManager;
        private readonly IAmazonListingService _amazonListingService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IPrepareForSyncAmazonListingService _prepareForSyncAmazonListingService;

        public ListingController(
            IAmazonListingSyncManager amazonListingSyncManager, 
            IAmazonListingService amazonListingService, 
            AmazonAppSettings amazonAppSettings, IPrepareForSyncAmazonListingService prepareForSyncAmazonListingService)
        {
            _amazonListingSyncManager = amazonListingSyncManager;
            _amazonListingService = amazonListingService;
            _amazonAppSettings = amazonAppSettings;
            _prepareForSyncAmazonListingService = prepareForSyncAmazonListingService;
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
            return View(_amazonListingService.GetAmazonListingModel(amazonListingGroup));
        }

        [HttpGet]
        public ActionResult ProductVariants(AmazonListingModel model)
        {
            return PartialView(_amazonListingService.GetAmazonListingModel(model));
        }

        [HttpGet]
        public ActionResult AddOne(string productVariantSku, int amazonListingGroupId)
        {
            if (!String.IsNullOrWhiteSpace(productVariantSku) && amazonListingGroupId > 0)
            {
                var amazonListing = _amazonListingService.GetByProductVariantSku(productVariantSku);
                if (amazonListing == null)
                {
                    amazonListing = _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(null, 
                        productVariantSku, amazonListingGroupId);
                    return View(amazonListing);
                }

                return RedirectToAction("Details", new {id = amazonListing.Id});
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("AddOne")]
        public ActionResult AddOne_POST(AmazonListing amazonListing)
        {
            if (amazonListing != null)
            {
                if (ModelState.IsValid)
                {
                    _amazonListingService.Save(amazonListing);
                    return RedirectToAction("SyncOne", new { id = amazonListing.Id });
                }
                return View(amazonListing);
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpGet]
        public ViewResult AddMany(AmazonListingGroup amazonListingGroup)
        {
            return View(new AmazonListingModel() { AmazonListingGroup = amazonListingGroup });
        }

        [HttpPost]
        [ActionName("AddMany")]
        public ActionResult AddMany_POST(AmazonListingModel model)
        {
            if (model != null && model.AmazonListingGroup!=null && !String.IsNullOrWhiteSpace(model.ChosenProductVariants))
            {
                _prepareForSyncAmazonListingService.InitAmazonListingsFromProductVariants(model.AmazonListingGroup, model.ChosenProductVariants);
                return RedirectToAction("SyncMany", new { id = model.AmazonListingGroup.Id });
            }
            return RedirectToAction("Index", "ListingGroup");
        }


        [HttpGet]
        [ActionName("SyncOne")]
        public ActionResult SyncOne_GET(AmazonListing amazonListing)
        {
            if (amazonListing != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                _prepareForSyncAmazonListingService.UpdateAmazonListing(amazonListing);
                return View(_amazonListingSyncManager.GetAmazonSyncModel(amazonListing));
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        public JsonResult SyncOne(AmazonSyncModel model)
        {
            if (model != null)
            {
                _amazonListingSyncManager.SyncAmazonListing(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        [ActionName("SyncMany")]
        public ActionResult SyncMany_GET(AmazonListingGroup amazonListingGroup)
        {
            if (amazonListingGroup != null)
            {
                _prepareForSyncAmazonListingService.UpdateAmazonListings(amazonListingGroup);
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                return View(_amazonListingSyncManager.GetAmazonSyncModel(amazonListingGroup));
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        public JsonResult SyncMany(AmazonSyncModel model)
        {
            if (model != null)
            {
                _amazonListingSyncManager.SyncAmazonListings(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult CloseOne(AmazonListing amazonListing)
        {
            if (amazonListing != null)
            {
                ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
                _prepareForSyncAmazonListingService.UpdateAmazonListing(amazonListing);
                return View(_amazonListingSyncManager.GetAmazonSyncModel(amazonListing));
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("CloseOne")]
        public JsonResult CloseOne_POST(AmazonSyncModel model)
        {
            if (model != null)
            {
                _amazonListingSyncManager.CloseAmazonListing(model);
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
                _prepareForSyncAmazonListingService.UpdateAmazonListings(amazonListingGroup);
                return View(_amazonListingSyncManager.GetAmazonSyncModel(amazonListingGroup));
            }
            return RedirectToAction("Index", "ListingGroup");
        }

        [HttpPost]
        [ActionName("CloseMany")]
        public JsonResult CloseMany_POST(AmazonSyncModel model)
        {
            if (model != null)
            {
                _amazonListingSyncManager.CloseAmazonListings(model);
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
