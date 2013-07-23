using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly ICartManager _cartManager;

        public PaymentDetailsController(CartModel cart, ICartManager cartManager)
        {
            _cart = cart;
            _cartManager = cartManager;
        }

        public ActionResult Show(PaymentDetails page)
        {
            if (_cart.ShippingMethod == null || _cart.ShippingAddress == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            if (_cart.Empty)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            ViewData["cart"] = _cart;
            ViewData["setting-billing-address"] = TempData["setting-billing-address"];
            return View(page);
        }

        [HttpPost]
        public RedirectResult BillingAddressSameAsShippingAddress(bool sameAsShipping)
        {
            _cartManager.SetBillingAddressSameAsShippingAddress(sameAsShipping);
            if (sameAsShipping)
                _cartManager.SetBillingAddress(null);
            else
                TempData["setting-billing-address"] = true;
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }

        [ChildActionOnly]
        public PartialViewResult SetBillingAddress()
        {
            return PartialView(_cart.BillingAddress);
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
    }
}