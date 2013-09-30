using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalRequestService : IPayPalRequestService
    {
        private readonly IPayPalUrlService _payPalUrlService;
        private readonly IPayPalShippingService _payPalShippingService;
        private readonly IPayPalOrderService _payPalOrderService;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalRequestService(IPayPalUrlService payPalUrlService,
                                    IPayPalShippingService payPalShippingService,
                                    IPayPalOrderService payPalOrderService,
                                    PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalUrlService = payPalUrlService;
            _payPalShippingService = payPalShippingService;
            _payPalOrderService = payPalOrderService;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public SetExpressCheckoutReq GetSetExpressCheckoutRequest(CartModel cart)
        {
            var setExpressCheckoutRequestDetailsType = new SetExpressCheckoutRequestDetailsType
                                                           {
                                                               ReturnURL = _payPalUrlService.GetReturnURL(),
                                                               CancelURL = _payPalUrlService.GetCancelURL(),
                                                               ReqConfirmShipping = _payPalShippingService.GetRequireConfirmedShippingAddress(),
                                                               NoShipping = _payPalShippingService.GetNoShipping(),
                                                               LocaleCode = _payPalExpressCheckoutSettings.LocaleCode,
                                                               cppHeaderImage = _payPalExpressCheckoutSettings.LogoImageURL,
                                                               cppCartBorderColor = _payPalExpressCheckoutSettings.CartBorderColor,
                                                               PaymentDetails = _payPalOrderService.GetPaymentDetails(cart),
                                                               BuyerEmail = _payPalOrderService.GetBuyerEmail(cart),
                                                               MaxAmount = _payPalOrderService.GetMaxAmount(cart),
                                                               InvoiceID = cart.CartGuid.ToString(),
                                                               Custom = cart.CartGuid.ToString()
                                                           };

            var setExpressCheckoutRequestType = new SetExpressCheckoutRequestType
                                                    {
                                                        SetExpressCheckoutRequestDetails = setExpressCheckoutRequestDetailsType,
                                                    };
            return new SetExpressCheckoutReq { SetExpressCheckoutRequest = setExpressCheckoutRequestType };
        }

        public GetExpressCheckoutDetailsReq GetGetExpressCheckoutRequest(string token)
        {
            return new GetExpressCheckoutDetailsReq
                       {
                           GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType { Token = token }
                       };
        }

        public DoExpressCheckoutPaymentReq GetDoExpressCheckoutRequest(CartModel cart)
        {
            // populate payment details
            var paymentDetails = new PaymentDetailsType
            {
                OrderTotal = cart.Total.GetAmountType(),
                ShippingTotal = cart.ShippingTotal.GetAmountType(),
                ItemTotal = (cart.Subtotal).GetAmountType(),
                Custom = cart.CartGuid.ToString(),
                ButtonSource = "mrcms-ecommerce",
                InvoiceID = cart.CartGuid.ToString()
            };
            // build the request
            return new DoExpressCheckoutPaymentReq
            {
                DoExpressCheckoutPaymentRequest = new DoExpressCheckoutPaymentRequestType
                {
                    DoExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType
                    {
                        Token = cart.PayPalExpressToken,
                        PayerID = cart.PayPalExpressPayerId,
                        PaymentAction = _payPalExpressCheckoutSettings.PaymentAction,
                        PaymentDetails = new List<PaymentDetailsType> { paymentDetails }
                    }
                }
            };
        }
    }
}