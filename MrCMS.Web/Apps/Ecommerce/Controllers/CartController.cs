using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICartManager _cartManager;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IDiscountManager _discountManager;
        private readonly IOrderShippingService _orderShippingService;
        private readonly CartModel _cart;

        public CartController(ICartManager cartManager,
            ICountryService countryService, IOrderService orderService, IDiscountManager discountManager,
            IOrderShippingService orderShippingService, CartModel cart)
        {
            _cartManager = cartManager;
            _countryService = countryService;
            _orderService = orderService;
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
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) && ValidateDiscountCode(model.DiscountCode))
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
        [ActionName("EditDiscountCode")]
        [HttpPost]
        public ActionResult EditDiscountCode_POST(CartModel model)
        {
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) && ValidateDiscountCode(model.DiscountCode))
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
                var discount = _discountManager.GetByCode(discountCode);
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
                var cartItem = _cart.Items.SingleOrDefault(x => x.Id == cartId);
                if (cartItem != null)
                    _cartManager.UpdateQuantity(cartItem, quantity);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult EnterOrderEmail()
        {
            return View(_cart);
        }
        [HttpPost]
        public RedirectResult EnterOrderEmail(string OrderEmail, bool havePassword = true)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(OrderEmail))
                {
                    _cartManager.SetOrderEmail(OrderEmail);
                    return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
                }
            }
            return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
        }

        [HttpPost]
        public RedirectResult SetDeliveryDetails(Address address)
        {
            if (address != null)
            {
                address.UserGuid = CurrentRequestData.UserGuid;
                _cartManager.SetShippingAddress(address);
                return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
            }
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }
        [HttpGet]
        public ActionResult PaymentDetails()
        {
            if (_cart.Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());

            var countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_cart.BillingAddress != null && _cart.BillingAddress.Country != null)
            {
                countries.SingleOrDefault(x => x.Value == _cart.BillingAddress.Country.Id.ToString()).Selected = true;
            }
            ViewData["countries"] = countries;

            if (_cart.ShippingAddress == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            return View(new PaymentDetailsModel() { CartModel = _cart });
        }
        [HttpPost]
        public ActionResult PaymentDetails(Address address, PaymentDetailsModel paymentDetailsModel)
        {
            if (address != null && !String.IsNullOrWhiteSpace(address.FirstName))
            {
                address.UserGuid = CurrentRequestData.UserGuid;
                _cartManager.SetBillingAddress(address);
            }
            else
            {
                _cartManager.SetBillingAddress(_cart.ShippingAddress);
            }
            if (paymentDetailsModel != null && !String.IsNullOrWhiteSpace(paymentDetailsModel.CartType))
            {
                _cartManager.SetPaymentMethod(paymentDetailsModel.CartType);
            }

            if (_cart.Items.Count > 0)
            {
                //TODO: IMPLEMENT PAYMENT GATEWAY PROCEDURES

                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>() + "?orderID=" + _orderService.PlaceOrder(_cart).ToString());
            }

            var countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_cart.BillingAddress != null && _cart.BillingAddress.Country != null)
            {
                countries.SingleOrDefault(x => x.Value == _cart.BillingAddress.Country.Id.ToString()).Selected = true;
            }
            ViewData["countries"] = countries;
            return View(_cart);
        }

    }
}