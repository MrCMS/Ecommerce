using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly ICartItemManager _cartItemManager;
        private readonly ICartValidationService _cartValidationService;

        public CartController(ICartItemManager cartItemManager, ICartValidationService cartValidationService,
            CartModel cart)
        {
            _cartItemManager = cartItemManager;
            _cartValidationService = cartValidationService;
            _cart = cart;
        }

        public ViewResult Show(Cart page)
        {
            SetupViewData();
            return View(page);
        }

        private void SetupViewData()
        {
            ViewData["cart"] = _cart;
        }

        [HttpGet]
        public PartialViewResult Details()
        {
            SetupViewData();
            return PartialView(_cart);
        }

        [HttpGet]
        public PartialViewResult HeaderSummary()
        {
            return PartialView(_cart);
        }

        [HttpGet]
        public PartialViewResult DiscountUseSummary()
        {
            return PartialView(_cart);
        }

        public JsonResult CanAddQuantity(AddToCartModel model)
        {
            CanAddQuantityValidationResult result = _cartValidationService.CanAddQuantity(model);
            return result.Valid
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(result.Message, JsonRequestBehavior.AllowGet);
        }

        [ActionName("DeleteCartItem")]
        [HttpPost]
        public JsonResult DeleteCartItem_POST(CartItem item)
        {
            _cartItemManager.Delete(item);
            return Json(true);
        }

        [HttpGet]
        public ViewResult DiscountCode()
        {
            return View(_cart);
        }

        [HttpGet]
        public ViewResult AddDiscountCode()
        {
            return View(_cart);
        }
        [HttpPost]
        public JsonResult UpdateBasket(
            [IoCModelBinder(typeof(UpdateBasketModelBinder))] List<CartUpdateValue> quantities)
        {
            _cartItemManager.UpdateQuantities(quantities);
            return Json(true);
        }
    }
}