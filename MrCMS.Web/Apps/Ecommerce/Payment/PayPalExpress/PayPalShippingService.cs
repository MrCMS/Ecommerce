using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalShippingService : IPayPalShippingService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalShippingService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public string GetRequireConfirmedShippingAddress()
        {
            return _payPalExpressCheckoutSettings.RequireConfirmedShippingAddress ? "1" : "0";
        }

        public string GetNoShipping(CartModel cart)
        {
            return !cart.RequiresShipping ? "1" : (cart.ShippingAddress != null ? "0" : "2");
        }
    }
}