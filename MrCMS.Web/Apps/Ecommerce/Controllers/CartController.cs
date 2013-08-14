using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

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

        [ActionName("DeleteCartItem")]
        [HttpPost]
        public JsonResult DeleteCartItem_POST(CartItem item)
        {
            _cartManager.Delete(item);
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
        public JsonResult ApplyDiscountCode(string discountCode)
        {
            if (String.IsNullOrWhiteSpace(discountCode))
            {
                _cartManager.SetDiscountCode(discountCode);
                return Json("Removed");
            }
            _cartManager.SetDiscountCode(discountCode);
            return Json(discountCode);
        }

        [HttpPost]
        public JsonResult UpdateBasket([IoCModelBinder(typeof(UpdateBasketModelBinder))] List<CartUpdateValue> quantities)
        {
            _cartManager.UpdateQuantities(quantities);
            return Json(true);
        }

        [HttpPost]
        public JsonResult EmptyBasket()
        {
            _cartManager.EmptyBasket();
            return Json(true);
        }
    }

    public class UpdateBasketModelBinder : MrCMSDefaultModelBinder
    {
        public UpdateBasketModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var cartUpdateValues = new List<CartUpdateValue>();

            var splitQuantities = (controllerContext.HttpContext.Request["quantities"] ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            splitQuantities.ForEach(s =>
                                        {
                                            var strings = s.Split(':');
                                            cartUpdateValues.Add(new CartUpdateValue
                                            {
                                                ItemId = Convert.ToInt32(strings[0]),
                                                Quantity = Convert.ToInt32(strings[1])
                                            });
                                        });
            return cartUpdateValues;
        }
    }
}