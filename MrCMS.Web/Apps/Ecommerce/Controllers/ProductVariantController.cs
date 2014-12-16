using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductVariantController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly ProductReviewSettings _productReviewSettings;
        private readonly IProductReviewUIService _productReviewUIService;
        private readonly ProductVariantService _productVariantService;
        private readonly IProductVariantUIService _productVariantUIService;

        public ProductVariantController(CartModel cart, IProductVariantUIService productVariantUIService,
            IProductReviewUIService productReviewUIService, ProductReviewSettings productReviewSettings,
            ProductVariantService productVariantService)
        {
            _cart = cart;
            _productVariantUIService = productVariantUIService;
            _productReviewUIService = productReviewUIService;
            _productReviewSettings = productReviewSettings;
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public JsonResult GetPriceBreaksForProductVariant(ProductVariant productVariant)
        {
            return productVariant != null
                ? Json(
                    productVariant.PriceBreaks.Select(priceBreak => new {priceBreak.Quantity, priceBreak.Price}),
                    JsonRequestBehavior.AllowGet)
                : Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Details(ProductVariant productVariant)
        {
            ViewData["cart"] = _cart;
            ViewData["can-buy-status"] = _productVariantUIService.CanBuyAny(productVariant);
            return PartialView(productVariant);
        }

        [HttpGet]
        public ActionResult ProductReviews(int productVariantId = 0, int reviewPage = 1)
        {
            int reviewsPageSize = _productReviewSettings.PageSize;
            ViewData["guest-reviews"] = _productReviewSettings.GuestReviews;
            ViewData["helpfulness-votes"] = _productReviewSettings.HelpfulnessVotes;
            ProductVariant productVariant = _productVariantService.Get(productVariantId);
            ViewData["reviews"] = _productReviewUIService.GetReviewsForVariant(productVariant, reviewPage,
                reviewsPageSize);
            ViewData["average-ratings"] = _productReviewUIService.GetAverageRatingForProductVariant(productVariant);

            return PartialView(productVariant);
        }
    }
}