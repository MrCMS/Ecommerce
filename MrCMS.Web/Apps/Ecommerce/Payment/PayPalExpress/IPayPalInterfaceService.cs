using System.Collections.Generic;
using Lucene.Net.Util;
using PayPal.Authentication;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalInterfaceService
    {
        SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq request);
    }

    public class PayPalInterfaceService : IPayPalInterfaceService
    {
        private readonly IPayPalSecurityService _payPalSecurityService;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly PayPalAPIInterfaceServiceService _payPalApiInterfaceServiceService;

        public PayPalInterfaceService(IPayPalSecurityService payPalSecurityService, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalSecurityService = payPalSecurityService;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            var config = new Dictionary<string, string>
                                 {
                                     {
                                         "mode", _payPalExpressCheckoutSettings.IsLive ? "live" : "sandbox"
                                     }
                                 };
            _payPalApiInterfaceServiceService = new PayPalAPIInterfaceServiceService(config);
        }

        public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq request)
        {
            return _payPalApiInterfaceServiceService.SetExpressCheckout(request, _payPalSecurityService.GetCredentials());
        }
    }
}