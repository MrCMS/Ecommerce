using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Wishlists;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class WishlistController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IWishlistUIService _wishlistUIService;
        private readonly CartModel _cartModel;

        public WishlistController(IWishlistUIService wishlistUIService, CartModel cartModel)
        {
            _wishlistUIService = wishlistUIService;
            _cartModel = cartModel;
        }

        public ActionResult Show(ShowWishlist page, string id)
        {
            if (!MrCMSApplication.Get<EcommerceSettings>().EnableWishlists)
                return Redirect("/");

            var wishlist = _wishlistUIService.GetWishlist(id);
            if (wishlist == null)
                return Redirect("~");
            ViewData["wishlist"] = wishlist;
            ViewData["my-wishlist"] = string.IsNullOrWhiteSpace(id);
            ViewData["cart"] = _cartModel;
            
            return View(page);
        }

        public PartialViewResult Add(ProductVariant productVariant)
        {
            ViewData["in-wishlist"] = _wishlistUIService.IsInWishlist(productVariant);
            ViewData["wisthlist-enabled"] = MrCMSApplication.Get<EcommerceSettings>().EnableWishlists;
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Add")]
        public JsonResult Add_POST(ProductVariant productVariant)
        {
            _wishlistUIService.Add(productVariant);
            return Json(true);
        }

        [HttpPost]
        [ActionName("Remove")]
        public JsonResult Remove(ProductVariant productVariant)
        {
            _wishlistUIService.Remove(productVariant);
            return Json(true);
        }

        public PartialViewResult Summary()
        {
            return PartialView(_wishlistUIService.GetSummary());
        }
    }
}