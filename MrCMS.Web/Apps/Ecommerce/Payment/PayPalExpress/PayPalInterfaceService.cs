using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalInterfaceService : IPayPalInterfaceService
    {
        private readonly IPayPalSecurityService _payPalSecurityService;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly IPayPalRequestService _payPalRequestService;
        private readonly PayPalAPIInterfaceServiceService _payPalApiInterfaceServiceService;

        public PayPalInterfaceService(IPayPalSecurityService payPalSecurityService, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, IPayPalRequestService payPalRequestService)
        {
            _payPalSecurityService = payPalSecurityService;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            _payPalRequestService = payPalRequestService;
            var config = new Dictionary<string, string>
                             {
                                 {
                                     "mode", _payPalExpressCheckoutSettings.IsLive ? "live" : "sandbox"
                                 }
                             };
            _payPalApiInterfaceServiceService = new PayPalAPIInterfaceServiceService(config);
        }

        public SetExpressCheckoutResponseType SetExpressCheckout(CartModel cart)
        {
            var request = _payPalRequestService.GetSetExpressCheckoutRequest(cart);
            return _payPalApiInterfaceServiceService.SetExpressCheckout(request, _payPalSecurityService.GetCredentials());
        }

        public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(string token)
        {
            var request = _payPalRequestService.GetGetExpressCheckoutRequest(token);
            return _payPalApiInterfaceServiceService.GetExpressCheckoutDetails(request, _payPalSecurityService.GetCredentials());
        }

        public DoExpressCheckoutPaymentResponseType DoExpressCheckout(CartModel cart)
        {
            var request = _payPalRequestService.GetDoExpressCheckoutRequest(cart);
            return _payPalApiInterfaceServiceService.DoExpressCheckoutPayment(request, _payPalSecurityService.GetCredentials());
        }
    }
}