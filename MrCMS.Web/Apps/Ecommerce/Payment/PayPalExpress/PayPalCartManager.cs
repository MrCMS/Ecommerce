using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalCartManager : IPayPalCartManager
    {
        private readonly ICartManager _cartManager;
        private readonly IShippingMethodUIService _shippingMethodUiService;
        private readonly PayPalExpressCheckoutPaymentMethod _payPalExpressCheckoutPaymentMethod;

        public PayPalCartManager(ICartManager cartManager, IShippingMethodUIService shippingMethodUiService,
            PayPalExpressCheckoutPaymentMethod payPalExpressCheckoutPaymentMethod)
        {
            _cartManager = cartManager;
            _shippingMethodUiService = shippingMethodUiService;
            _payPalExpressCheckoutPaymentMethod = payPalExpressCheckoutPaymentMethod;
        }

        public bool UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            _cartManager.SetPaymentMethod(_payPalExpressCheckoutPaymentMethod.SystemName);
            _cartManager.SetPayPalExpressPayerId(details.PayerInfo.PayerID);

            _cartManager.SetBillingAddress(details.BillingAddress.GetAddress());

            var paymentDetails = details.PaymentDetails.FirstOrDefault();
            if (paymentDetails != null)
            {
                _cartManager.SetShippingAddress(paymentDetails.ShipToAddress.GetAddress());
            }
            var userSelectedOptions = details.UserSelectedOptions;
            var shippingMethodSet = false;
            if (userSelectedOptions != null)
            {
                var shippingOptionName = userSelectedOptions.ShippingOptionName;
                var enabledMethods = _shippingMethodUiService.GetEnabledMethods();
                var shippingMethod = enabledMethods.FirstOrDefault(method => method.TypeName == shippingOptionName) ??
                                     enabledMethods.FirstOrDefault(method => method.Name == shippingOptionName) ??
                                     enabledMethods.FirstOrDefault(method => method.DisplayName == shippingOptionName);
                if (shippingMethod != null)
                {
                    _cartManager.SetShippingMethod(shippingMethod);
                    shippingMethodSet = true;
                }
            }
            return shippingMethodSet;
        }
    }
}