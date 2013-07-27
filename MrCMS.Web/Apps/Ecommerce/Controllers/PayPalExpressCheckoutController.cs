using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
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
        private readonly ICartBuilder _cartBuilder;
        private readonly IDocumentService _documentService;

        public PayPalExpressCheckoutController(IPayPalExpressService payPalExpressService, ICartBuilder cartBuilder, IDocumentService documentService)
        {
            _payPalExpressService = payPalExpressService;
            _cartBuilder = cartBuilder;
            _documentService = documentService;
        }

        [HttpPost]
        public ActionResult SetExpressCheckout()
        {
            var response = _payPalExpressService.GetSetExpressCheckoutRedirectUrl(_cartBuilder.BuildCart());

            return response.Success
                       ? Redirect(response.Url)
                       : RedirectTo<Cart>();
        }

        public ActionResult Return(string token)
        {
            var response = _payPalExpressService.ProcessReturn(token);

            return response.Success
                       ? RedirectTo<SetDeliveryDetails>()
                       : RedirectTo<Cart>();
        }

        private ActionResult RedirectTo<T>() where T : Webpage, IUniquePage
        {
            var page = _documentService.GetUniquePage<T>();
            return Redirect(page != null
                                ? string.Format("/{0}", page.LiveUrlSegment)
                                : "/");
        }
    }
}