using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public PaymentDetailsController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public ActionResult Show(PaymentDetails page)
        {
            if (_getCart.GetShippingMethod() == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            if (String.IsNullOrWhiteSpace(_getCart.GetOrderEmail()))
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            return View(page);
        }
    }
}