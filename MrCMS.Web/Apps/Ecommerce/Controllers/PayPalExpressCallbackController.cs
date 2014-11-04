using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.ActionResults;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PayPalExpressCallbackController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetPaypalShippingOptions _getPaypalShippingOptions;

        public PayPalExpressCallbackController(IGetPaypalShippingOptions getPaypalShippingOptions)
        {
            _getPaypalShippingOptions = getPaypalShippingOptions;
        }

        public ActionResult Handler(PaypalShippingInfo info)
        {
            var options = _getPaypalShippingOptions.Get(info);
            return new CallbackResponseResult(options);
        }
    }
}