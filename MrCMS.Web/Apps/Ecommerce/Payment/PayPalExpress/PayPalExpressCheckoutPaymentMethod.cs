using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressCheckoutPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "PayPal Express Checkout"; }
        }

        public string SystemName
        {
            get { return "PayPalExpressCheckout"; }
        }

        public string ControllerName
        {
            get { return "PayPalExpressCheckout"; }
        }

        public string ActionName
        {
            get { return "Form"; }
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.PayPalExpress; }
        }

        public bool Enabled
        {
            get { return MrCMSApplication.Get<PayPalExpressCheckoutSettings>().Enabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return cart.IsPayPalTransaction;
        }
    }
}