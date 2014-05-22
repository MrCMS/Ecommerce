using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressCheckoutPaymentMethod : BasePaymentMethod
    {
        public override string Name
        {
            get { return "PayPal Express Checkout"; }
        }

        public override string SystemName
        {
            get { return "PayPalExpressCheckout"; }
        }

        public override string ControllerName
        {
            get { return "PayPalExpressCheckout"; }
        }

        public override string ActionName
        {
            get { return "Form"; }
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.PayPalExpress; }
        }

        public override bool Enabled
        {
            get { return MrCMSApplication.Get<PayPalExpressCheckoutSettings>().Enabled; }
        }

        public override bool CanUse(CartModel cart)
        {
            return cart.IsPayPalTransaction;
        }
    }
}