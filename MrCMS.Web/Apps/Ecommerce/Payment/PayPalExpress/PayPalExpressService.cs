using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressService : IPayPalExpressService
    {
        private readonly IPayPalUrlService _payPalUrlService;
        private readonly IPayPalInterfaceService _payPalInterfaceService;
        private readonly IPayPalCartManager _payPalCartManager;
        private readonly ICartManager _cartManager;

        public PayPalExpressService(IPayPalUrlService payPalUrlService, IPayPalInterfaceService payPalInterfaceService, IPayPalCartManager payPalCartManager, ICartManager cartManager)
        {
            _payPalUrlService = payPalUrlService;
            _payPalInterfaceService = payPalInterfaceService;
            _payPalCartManager = payPalCartManager;
            _cartManager = cartManager;
        }

        public SetExpressCheckoutResponse GetSetExpressCheckoutRedirectUrl(CartModel cart)
        {
            SetExpressCheckoutResponseType setExpressCheckoutResponseType = _payPalInterfaceService.SetExpressCheckout(cart);

            return setExpressCheckoutResponseType
                .HandleResponse<SetExpressCheckoutResponseType, SetExpressCheckoutResponse>(
                    (type, response) =>
                    {
                        response.Url = _payPalUrlService.GetExpressCheckoutRedirectUrl(type.Token);
                        _cartManager.SetPayPalExpressToken(type.Token);
                    },
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
                ((type, response) =>
                {
                    response.AddressAndMethodSet = _payPalCartManager.UpdateCart(type.GetExpressCheckoutDetailsResponseDetails);

                },
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
                        if (type.Errors.Any(errorType => errorType.ErrorCode == "10486" || errorType.ErrorCode == "10411"))
                        {
                            response.RedirectToPayPal = true;
                        }
                    });
        }

        public void Reset()
        {
            _cartManager.ResetPayPalExpress();
        }
    }
}