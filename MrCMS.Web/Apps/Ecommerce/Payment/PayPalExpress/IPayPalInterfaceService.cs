using System.Collections.Generic;
using Lucene.Net.Util;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using PayPal.Authentication;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalInterfaceService
    {
        SetExpressCheckoutResponseType SetExpressCheckout(CartModel cart);
        GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(string token);
    }

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
    }
}