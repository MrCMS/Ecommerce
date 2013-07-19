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
        private readonly PayPalAPIInterfaceServiceService _payPalApiInterfaceServiceService;

        public PayPalInterfaceService(IPayPalSecurityService payPalSecurityService)
        {
            _payPalSecurityService = payPalSecurityService;
            _payPalApiInterfaceServiceService = new PayPalAPIInterfaceServiceService();
        }

        public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq request)
        {
            return _payPalApiInterfaceServiceService.SetExpressCheckout(request, _payPalSecurityService.GetCredentials());
        }
    }
}