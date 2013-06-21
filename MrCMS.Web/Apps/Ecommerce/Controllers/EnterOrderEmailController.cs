using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Helpers;
using System;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class EnterOrderEmailController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public EnterOrderEmailController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public ActionResult Show(EnterOrderEmail page)
        {
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            if (String.IsNullOrWhiteSpace(_getCart.GetOrderEmail()) && CurrentRequestData.CurrentUser != null)
                _getCart.SetOrderEmail(CurrentRequestData.CurrentUser.Email);
            if (!String.IsNullOrWhiteSpace(_getCart.GetOrderEmail()) && CurrentRequestData.CurrentUser != null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            return View(page);
        }
    }
}