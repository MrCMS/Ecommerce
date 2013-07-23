using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;

        public PaymentDetailsController(CartModel cart)
        {
            _cart = cart;
        }

        public ActionResult Show(PaymentDetails page)
        {
            if (_cart.ShippingMethod == null || _cart.ShippingAddress == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            if (_cart.Empty)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            return View(page);
        }
    }
}