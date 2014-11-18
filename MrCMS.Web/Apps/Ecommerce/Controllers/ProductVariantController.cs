using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IProductVariantUIService _productVariantUIService;
        private readonly IReviewService _reviewService;

        public ProductVariantController(CartModel cart, IProductVariantUIService productVariantUIService, IReviewService reviewService)
        {
            _cart = cart;
            _productVariantUIService = productVariantUIService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(ProductVariant productVariant)
        {
            return productVariant != null
                       ? Json(
                           productVariant.PriceBreaks.Select(priceBreak => new { priceBreak.Quantity, priceBreak.Price }),
                           JsonRequestBehavior.AllowGet)
                       : Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Details(ProductVariant productVariant)
        {
            ViewData["cart"] = _cart;
            ViewData["can-buy-status"] = _productVariantUIService.CanBuyAny(productVariant);
            return PartialView(productVariant);
        }

        public ActionResult ProductReviews(ProductVariant productVariant, int reviewPage = 1)
        {
            var reviewsPageSize = MrCMSApplication.Get<ProductReviewSettings>().PageSize;
            ViewData["guest-reviews"] = MrCMSApplication.Get<ProductReviewSettings>().GuestReviews;
            ViewData["helpfulness-votes"] = MrCMSApplication.Get<ProductReviewSettings>().HelpfulnessVotes;
            ViewData["reviews"] = _reviewService.GetReviewsByProductVariantId(productVariant, reviewPage, reviewsPageSize);
            ViewData["average-ratings"] = _reviewService.GetAverageRatingsByProductVariant(productVariant);

            return PartialView(productVariant);
        }
    }
}