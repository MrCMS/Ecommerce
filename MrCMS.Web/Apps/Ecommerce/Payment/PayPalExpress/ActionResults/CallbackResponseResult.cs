using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.ActionResults
{
    public class CallbackResponseResult : ContentResult
    {
        private readonly List<PaypalShippingOption> _options;

        public CallbackResponseResult(List<PaypalShippingOption> options)
        {
            _options = options;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var encoder = new NVPCodec();

            encoder["METHOD"] = "CallbackResponse";
            encoder["CURRENCYCODE"] = MrCMSApplication.Get<PayPalExpressCheckoutSettings>().Currency.ToString();

            for (int index = 0; index < _options.Count; index++)
            {
                var option = _options[index];
                encoder[string.Format("L_SHIPPINGOPTIONNAME{0}", index)] = option.DisplayName;
                encoder[string.Format("L_SHIPPINGOPTIONLABEL{0}", index)] = option.Label;
                encoder[string.Format("L_SHIPPINGOPTIONAMOUNT{0}", index)] = option.Amount.ToString("0.00");
                encoder[string.Format("L_TAXAMT{0}", index)] = option.TotalTax.ToString("0.00");
                encoder[string.Format("L_SHIPPINGOPTIONISDEFAULT{0}", index)] = (option.Default ? "true" : "false");
            }

            context.HttpContext.Response.Write(encoder.Encode());
        }
    }
}