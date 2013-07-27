using MrCMS.Web.Apps.Ecommerce.Models;
using PayPal.PayPalAPIInterfaceService.Model;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressService : IPayPalExpressService
    {
        private readonly IPayPalUrlService _payPalUrlService;
        private readonly IPayPalInterfaceService _payPalInterfaceService;
        private readonly IPayPalCartManager _payPalCartManager;

        public PayPalExpressService(IPayPalUrlService payPalUrlService, IPayPalInterfaceService payPalInterfaceService, IPayPalCartManager payPalCartManager)
        {
            _payPalUrlService = payPalUrlService;
            _payPalInterfaceService = payPalInterfaceService;
            _payPalCartManager = payPalCartManager;
        }

        public SetExpressCheckoutResponse GetSetExpressCheckoutRedirectUrl(CartModel cart)
        {
            SetExpressCheckoutResponseType setExpressCheckoutResponseType = _payPalInterfaceService.SetExpressCheckout(cart);

            return setExpressCheckoutResponseType
                .HandleResponse<SetExpressCheckoutResponseType, SetExpressCheckoutResponse>(
                    (type, response) => { response.Url = _payPalUrlService.GetExpressCheckoutRedirectUrl(type.Token); },
                    (type, response) =>
                        {
                            response.Errors.Add("An error occurred");
                            type.RaiseErrors();
                        });
        }

        public GetExpressCheckoutResponse ProcessReturn(string token)
        {
            var getExpressCheckoutDetailsResponseType = _payPalInterfaceService.GetExpressCheckoutDetails(token);

            return getExpressCheckoutDetailsResponseType
                .HandleResponse<GetExpressCheckoutDetailsResponseType, GetExpressCheckoutResponse>
                ((type, response) => _payPalCartManager.UpdateCart(type.GetExpressCheckoutDetailsResponseDetails),
                 (type, response) =>
                     {
                         response.Errors.Add("An error occurred");
                         type.RaiseErrors();
                     });
        }

        public DoExpressCheckoutPaymentResponse DoExpressCheckout(CartModel cart)
        {
            var doExpressCheckoutPaymentResponseType = _payPalInterfaceService.DoExpressCheckout(cart);

            return doExpressCheckoutPaymentResponseType
                .HandleResponse<DoExpressCheckoutPaymentResponseType, DoExpressCheckoutPaymentResponse>(
                    (type, response) =>
                        {
                            response.Details = type.DoExpressCheckoutPaymentResponseDetails;
                        },
                    (type, response) =>
                        {
                            response.Errors.Add("An error occurred");
                            type.RaiseErrors();
                        });
        }
    }
}