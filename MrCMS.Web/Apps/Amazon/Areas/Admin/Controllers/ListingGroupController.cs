using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class ListingGroupController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonListingGroupService _amazonListingGroupService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly SiteSettings _siteSettings;

        public ListingGroupController(IAmazonListingGroupService amazonListingGroupService, 
            AmazonAppSettings amazonAppSettings, SiteSettings siteSettings)
        {
            _amazonListingGroupService = amazonListingGroupService;
            _amazonAppSettings = amazonAppSettings;
            _siteSettings = siteSettings;
        }

        [HttpGet]
        public ViewResult Index(string searchTerm,int page = 1)
        {
            ViewData["AmazonManageInventoryUrl"] = _amazonAppSettings.AmazonManageInventoryUrl;
            var results = _amazonListingGroupService.Search(searchTerm, page, _siteSettings.DefaultPageSize);
            return View(results);
        }

        [HttpGet]
        public PartialViewResult ListingGroups(string name, int page = 1)
        {
            var results = _amazonListingGroupService.Search(name, page, _siteSettings.DefaultPageSize);
            return PartialView(results);
        }

        [HttpGet]
        public PartialViewResult Listings(AmazonListingGroup amazonListingGroup,int page = 1)
        {
            ViewData["AmazonProductDetailsUrl"] = _amazonAppSettings.AmazonProductDetailsUrl;
            var results = new PagedList<AmazonListing>(amazonListingGroup.Items, page, _siteSettings.DefaultPageSize);
            return PartialView(results);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Add")]
        public ActionResult Add_POST(AmazonListingGroup amazonListingGroup)
        {
            if (ModelState.IsValid)
            {
                _amazonListingGroupService.Save(amazonListingGroup);
                return RedirectToAction("Edit", "ListingGroup", new { id = amazonListingGroup.Id });
            }
            return PartialView(amazonListingGroup);
        }

        [HttpGet]
        public ActionResult Edit(AmazonListingGroup amazonListingGroup)
        {
            if (amazonListingGroup != null)
                return View(amazonListingGroup);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_POST(AmazonListingGroup amazonListingGroup)
        {
            ViewData["AmazonProductDetailsUrl"] = _amazonAppSettings.AmazonProductDetailsUrl;
            if (ModelState.IsValid)
            {
                _amazonListingGroupService.Save(amazonListingGroup);
                return RedirectToAction("Edit", "ListingGroup", new { id = amazonListingGroup.Id });
            }
            return PartialView(amazonListingGroup);
        }

        [HttpGet]
        public ActionResult Delete(AmazonListingGroup amazonListingGroup)
        {
            if (amazonListingGroup != null)
                return View(amazonListingGroup);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(AmazonListingGroup amazonListingGroup)
        {
            _amazonListingGroupService.Delete(amazonListingGroup);
            return RedirectToAction("Index");
        }
    }
}
