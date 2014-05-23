using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Payment.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IOrderShippingService _orderShippingService;
        private readonly ICartManager _cartManager;

        public PaymentDetailsController(CartModel cart, IOrderShippingService orderShippingService, ICartManager cartManager)
        {
            _cart = cart;
            _orderShippingService = orderShippingService;
            _cartManager = cartManager;
        }

        public ActionResult Show(PaymentDetails page)
        {
            if (_cart.RequiresShipping && (_cart.ShippingMethod == null || _cart.ShippingAddress == null))
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            if (_cart.Empty)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            ViewData["cart"] = _cart;
            ViewData["message"] = TempData["message"];

            ViewData["setting-billing-address"] = (TempData["setting-billing-address"] is bool &&
                                                   (bool)TempData["setting-billing-address"]) ||
                                                  (!_cart.BillingAddressSameAsShippingAddress &&
                                                   _cart.BillingAddress == null);

            return View(page);
        }

        [HttpPost]
        public RedirectResult BillingAddressSameAsShippingAddress(bool sameAsShipping)
        {
            _cartManager.SetBillingAddressSameAsShippingAddress(sameAsShipping);

            SetSettingBillingAddress(sameAsShipping);
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }

        private void SetSettingBillingAddress(bool sameAsShipping)
        {
            if (sameAsShipping && _cart.BillingAddress != null)
                _cartManager.SetBillingAddress(null);
            else
                TempData["setting-billing-address"] = true;
        }

        [ChildActionOnly]
        public PartialViewResult SetBillingAddress()
        {
            ViewData["other-addresses"] = _orderShippingService.ExistingAddressOptions(_cart, _cart.BillingAddress);
            return PartialView(_cart.BillingAddress);
        }

        [ChildActionOnly]
        public PartialViewResult BillingAddress()
        {
            SetSettingBillingAddress(_cart.BillingAddressSameAsShippingAddress);
            ViewData["setting-billing-address"] = TempData["setting-billing-address"];
            return PartialView(_cart);
        }

        [HttpPost]
        public RedirectResult UpdateBillingAddress()
        {
            TempData["setting-billing-address"] = true;
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }

        [HttpPost]
        public RedirectResult SaveBillingAddress(Address address)
        {
            _cartManager.SetBillingAddress(address);
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }

        [HttpPost]
        public JsonResult SetPaymentMethod(string paymentMethod)
        {
            var method = _cartManager.SetPaymentMethod(paymentMethod);
            return Json(method.GetUrl(Url));
        }
    }
}