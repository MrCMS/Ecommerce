using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalCartManager : IPayPalCartManager
    {
        private readonly CartModel _cart;
        private readonly ICartManager _cartManager;
        private readonly IShippingMethodUIService _shippingMethodUiService;

        public PayPalCartManager(ICartManager cartManager, IShippingMethodUIService shippingMethodUiService,
            CartModel cart)
        {
            _cartManager = cartManager;
            _shippingMethodUiService = shippingMethodUiService;
            _cart = cart;
        }

        public bool UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            _cartManager.SetPaymentMethod(PayPalExpressCheckoutPaymentMethod.MethodSystemName);
            _cartManager.SetPayPalExpressPayerId(details.PayerInfo.PayerID);
            _cartManager.SetBillingAddress(details.BillingAddress.GetAddress());

            SetEmail(details);

            SetShippingAddress(details);

            return SetShippingMethod(details);
        }

        private bool SetShippingMethod(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            UserSelectedOptionType userSelectedOptions = details.UserSelectedOptions;
            bool shippingMethodSet = false;
            if (userSelectedOptions != null)
            {
                string shippingOptionName = userSelectedOptions.ShippingOptionName;
                if (string.IsNullOrWhiteSpace(shippingOptionName))
                    return false;
                HashSet<IShippingMethod> enabledMethods = _shippingMethodUiService.GetEnabledMethods();
                IShippingMethod shippingMethod =
                    enabledMethods.FirstOrDefault(method => shippingOptionName.StartsWith(method.TypeName)) ??
                    enabledMethods.FirstOrDefault(method => shippingOptionName.StartsWith(method.Name)) ??
                    enabledMethods.FirstOrDefault(method => shippingOptionName.StartsWith(method.DisplayName));
                if (shippingMethod != null)
                {
                    _cartManager.SetShippingMethod(shippingMethod);
                    shippingMethodSet = true;
                }
            }
            return shippingMethodSet;
        }

        private void SetShippingAddress(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            PaymentDetailsType paymentDetails = details.PaymentDetails.FirstOrDefault();
            if (paymentDetails != null)
            {
                _cartManager.SetShippingAddress(paymentDetails.ShipToAddress.GetAddress());
            }
        }

        private void SetEmail(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
            {
                PayerInfoType payer = details.PayerInfo;
                if (payer != null && !string.IsNullOrWhiteSpace(payer.Payer))
                {
                    _cartManager.SetOrderEmail(payer.Payer);
                }
            }
        }
    }
}