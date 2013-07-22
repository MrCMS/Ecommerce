using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PayPalExpressCheckoutController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPayPalExpressService _payPalExpressService;
        private readonly IGetCart _getCart;
        private readonly IDocumentService _documentService;

        public PayPalExpressCheckoutController(IPayPalExpressService payPalExpressService, IGetCart getCart, IDocumentService documentService)
        {
            _payPalExpressService = payPalExpressService;
            _getCart = getCart;
            _documentService = documentService;
        }

        [HttpPost]
        public ActionResult SetExpressCheckout()
        {
            var response = _payPalExpressService.GetSetExpressCheckoutRedirectUrl(_getCart.GetCart());
            if (response.Success)
                return Redirect(response.Url);

            var cart = _documentService.GetUniquePage<Cart>();
            return Redirect(cart != null
                                ? string.Format("/{0}", cart.LiveUrlSegment)
                                : "/");
        }
    }
}