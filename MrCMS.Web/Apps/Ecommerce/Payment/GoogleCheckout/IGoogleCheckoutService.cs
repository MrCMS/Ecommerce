using GCheckout.Checkout;
using GCheckout.Util;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.GoogleCheckout
{
    public interface IGoogleCheckoutService
    {
        string GetRedirectUrl(CartModel cart);
    }

    public class GoogleCheckoutService : IGoogleCheckoutService
    {
        private readonly PaymentSettings _paymentSettings;

        public GoogleCheckoutService(PaymentSettings paymentSettings)
        {
            _paymentSettings = paymentSettings;
        }

        public string GetRedirectUrl(CartModel cart)
        {
            var checkoutShoppingCartRequest =
                new CheckoutShoppingCartRequest(_paymentSettings.GoogleCheckoutMerchantID,
                                                _paymentSettings.GoogleCheckoutMerchantKey,
                                                _paymentSettings.GoogleCheckoutEnvironment, "GBP", 0, false);
            return null;
        }
    }
}