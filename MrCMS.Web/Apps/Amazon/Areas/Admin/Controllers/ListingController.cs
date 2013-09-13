using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class ListingController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IOptionService _optionService;
        private readonly IProductService _productService;

        public ListingController(IOptionService optionService,
            IProductService productService)
        {
            _optionService = optionService;
            _productService = productService;
        }

        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            //var model = _eBayListingManager.GetEntriesPaged(page);
            return View();
        }

        //[HttpGet]
        //public ActionResult Listings(string listingTitle, int page = 1)
        //{
        //    var listings = _eBayListingManager.SearchListings(listingTitle, page);
        //    return PartialView(listings);
        //}

        //[HttpGet]
        //public ViewResult Add()
        //{
        //    ViewData["categories"] = _optionService.GetCategoryOptions();
        //    var model = new AmazonListingModel() { Products = _productService.Search(string.Empty, 1) };
        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult AddListingItems(Product product)
        //{
        //    if (product != null)
        //    {
        //        var model = new AmazonListingModel()
        //        {
        //            ChosenProduct = product,
        //            //Categories = _eBayCategoryService.Search(string.Empty)
        //        };
        //        return View(model);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //[ActionName("AddListingItems")]
        //public ActionResult AddListingItems_POST(AmazonListingModel model)
        //{
        //    if (model != null && model.ChosenProduct != null)
        //    {
        //        if (model.ChosenCategory != null)
        //        {
        //            var listing = _eBayListingManager.AddItemsToListing(model.ChosenProduct, model.ChosenCategory, model.MultipleVariations);

        //            return RedirectToAction("Edit", new { id = listing.Id });
        //        }
        //        return RedirectToAction("AddListingItems", new { id = model.ChosenProduct.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public JsonResult CheckIfCategoryAllowsVariations(AmazonCategory category)
        //{
        //    if (category != null)
        //    {
        //        var variationsEnabled = _eBayApiService.CheckIfCategorySupportsVariations(category);

        //        return Json(variationsEnabled);
        //    }
        //    return Json(false);
        //}

        //[HttpGet]
        //public ActionResult Products(AmazonListingModel model)
        //{
        //    var newModel = new AmazonListingModel() { Products = _productService.Search(model.Name, model.Page) };
        //    return PartialView(newModel);
        //}

        //[HttpGet]
        //public ActionResult Categories(AmazonListingModel model)
        //{
        //    var newModel = new AmazonListingModel() { Categories = _eBayCategoryService.Search(model.Name, model.Page, model.PageSize) };
        //    return PartialView(newModel);
        //}

        //[HttpGet]
        //public ActionResult End(AmazonListing listing, string location)
        //{
        //    TempData["Location"] = location;
        //    return View(listing);
        //}

        //[HttpPost]
        //[ActionName("End")]
        //public ActionResult End_POST(AmazonListing listing, string location)
        //{
        //    if (listing != null)
        //    {
        //        _eBaySyncListingsService.EndListings(listing);
        //        return string.IsNullOrWhiteSpace(location) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = listing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult Delete(AmazonListing listing, string location)
        //{
        //    TempData["Location"] = location;
        //    return View(listing);
        //}

        //[HttpPost]
        //[ActionName("Delete")]
        //public ActionResult Delete_POST(AmazonListing listing, string location)
        //{
        //    if (listing != null)
        //    {
        //        _eBaySyncListingsService.EndListings(listing);
        //        _eBayListingManager.DeleteListing(listing);
        //        return string.IsNullOrWhiteSpace(location) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = listing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult ListAll(AmazonListing listing, string location)
        //{
        //    TempData["Location"] = location;
        //    return View(listing);
        //}

        //[HttpPost]
        //[ActionName("ListAll")]
        //public ActionResult ListAll_POST(AmazonListing listing, string location)
        //{
        //    if (listing != null)
        //    {
        //        _eBaySyncListingsService.ListItems(listing);
        //        return string.IsNullOrWhiteSpace(location) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = listing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult Sync(AmazonListing listing, string location)
        //{
        //    TempData["Location"] = location;
        //    return View(listing);
        //}

        //[HttpPost]
        //[ActionName("Sync")]
        //public ActionResult Sync_POST(AmazonListing listing, string location)
        //{
        //    if (listing != null)
        //    {
        //        _eBaySyncListingsService.SyncListing(listing);
        //        return string.IsNullOrWhiteSpace(location) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = listing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult Edit(AmazonListing listing)
        //{
        //    if (listing != null)
        //    {
        //        return View(listing);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult ListingItems(AmazonListing listing, int page = 1)
        //{
        //    if (listing != null)
        //    {
        //        return PartialView(new PagedList<AmazonListingItem>(listing.Items, page, 10));
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult ListListingItem(AmazonListingItem listingItem)
        //{
        //    if (listingItem != null)
        //    {
        //        _eBaySyncListingsService.ListItem(listingItem);
        //        return RedirectToAction("Edit", new { id = listingItem.AmazonListing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult EndListingItem(AmazonListingItem listingItem)
        //{
        //    if (listingItem != null)
        //    {
        //        _eBaySyncListingsService.EndListing(listingItem);
        //        return RedirectToAction("Edit", new { id = listingItem.AmazonListing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult RelistListingItem(AmazonListingItem listingItem)
        //{
        //    if (listingItem != null)
        //    {
        //        _eBaySyncListingsService.RelistItem(listingItem);
        //        return RedirectToAction("Edit", new { id = listingItem.AmazonListing.Id });
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}
