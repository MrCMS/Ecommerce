using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly ICartManager _cartManager;
        private readonly IDiscountManager _discountManager;
        private readonly IOrderShippingService _orderShippingService;

        public CartController(ICartManager cartManager, IDiscountManager discountManager,
                              IOrderShippingService orderShippingService, CartModel cart)
        {
            _cartManager = cartManager;
            _discountManager = discountManager;
            _orderShippingService = orderShippingService;
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
            ViewData["shipping-calculations"] = _orderShippingService.GetShippingOptions(_cart);
        }

        [HttpGet]
        public ViewResult CartPanel()
        {
            return View(_cart);
        }

        [HttpGet]
        public PartialViewResult Details()
        {
            SetupViewData();
            return PartialView(_cart);
        }

        [HttpPost]
        public RedirectResult AddToCart(ProductVariant productVariant, int quantity = 0)
        {
            if (productVariant != null && quantity > 0)
            {
                _cartManager.AddToCart(productVariant, quantity);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
        }

        [HttpGet]
        public ViewResult EditCartItem(CartItem item)
        {
            return View("EditCartItem", item);
        }

        [ActionName("EditCartItem")]
        [HttpPost]
        public ActionResult EditCartItem_POST(CartItem item)
        {
            if (ModelState.IsValid)
            {
                _cartManager.UpdateQuantity(item, item.Quantity);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(item);
        }

        [HttpGet]
        public PartialViewResult DeleteCartItem(CartItem item)
        {
            return PartialView(item);
        }

        [ActionName("DeleteCartItem")]
        [HttpPost]
        public ActionResult DeleteCartItem_POST(CartItem item)
        {
            _cartManager.Delete(item);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
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

        [ActionName("AddDiscountCode")]
        [HttpPost]
        public ActionResult AddDiscountCode_POST(CartModel model)
        {
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) &&
                ValidateDiscountCode(model.DiscountCode))
            {
                _cartManager.SetDiscountCode(model.DiscountCode);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(_cart);
        }

        [HttpPost]
        public JsonResult AddDiscountCodeAjax(string discountCode)
        {
            if (String.IsNullOrWhiteSpace(discountCode))
            {
                _cartManager.SetDiscountCode(discountCode);
                return Json("Removed");
            }
            else if (!String.IsNullOrWhiteSpace(discountCode) && ValidateDiscountCode(discountCode))
            {
                _cartManager.SetDiscountCode(discountCode);
                return Json(discountCode);
            }
            return Json(false);
        }

        [HttpGet]
        public ViewResult EditDiscountCode()
        {
            return View(_cart);
        }

        [HttpPost]
        public ActionResult EditDiscountCode(CartModel model)
        {
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) &&
                ValidateDiscountCode(model.DiscountCode))
            {
                _cartManager.SetDiscountCode(model.DiscountCode);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(_cart);
        }

        public JsonResult IsDiscountCodeValid(string discountCode)
        {
            return Json(ValidateDiscountCode(discountCode), JsonRequestBehavior.AllowGet);
        }

        private bool ValidateDiscountCode(string discountCode)
        {
            if (!String.IsNullOrWhiteSpace(discountCode))
            {
                Discount discount = _discountManager.GetByCode(discountCode);
                if (discount != null)
                {
                    if (discount.IsCodeValid(discountCode))
                        return true;
                }
            }

            return false;
        }

        [HttpPost]
        public JsonResult UpdateQuantity(int quantity = 0, int cartId = 0)
        {
            if (quantity > 0 && cartId != 0)
            {
                CartItem cartItem = _cart.Items.SingleOrDefault(x => x.Id == cartId);
                if (cartItem != null)
                    _cartManager.UpdateQuantity(cartItem, quantity);
                return Json(true);
            }
            return Json(false);
        }
    }
}