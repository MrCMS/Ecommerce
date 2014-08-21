using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalCartManager : IPayPalCartManager
    {
        private readonly ICartManager _cartManager;
        private readonly IShippingMethodUIService _shippingMethodUiService;

        public PayPalCartManager(ICartManager cartManager, IShippingMethodUIService shippingMethodUiService)
        {
            _cartManager = cartManager;
            _shippingMethodUiService = shippingMethodUiService;
        }

        public void UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            _cartManager.SetPaymentMethod(new PayPalExpressCheckoutPaymentMethod().SystemName);
            _cartManager.SetPayPalExpressPayerId(details.PayerInfo.PayerID);

            _cartManager.SetBillingAddress(details.BillingAddress.GetAddress());

            var paymentDetails = details.PaymentDetails.FirstOrDefault();
            if (paymentDetails != null)
            {
                var userSelectedOptions = details.UserSelectedOptions;
                if (userSelectedOptions != null)
                    _cartManager.SetShippingMethod(_shippingMethodUiService.GetMethodByTypeName(userSelectedOptions.ShippingOptionName));
                _cartManager.SetShippingAddress(paymentDetails.ShipToAddress.GetAddress());
            }
        }
    }
}