using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery
{
    public class CashOnDeliveryPaymentMethod : BasePaymentMethod
    {
        public override string Name
        {
            get { return "Cash On Delivery"; }
        }

        public override string SystemName
        {
            get { return "CashOnDelivery"; }
        }

        public override string ControllerName
        {
            get { return "CashOnDelivery"; }
        }

        public override string ActionName
        {
            get { return "Form"; }
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.ServiceBased; }
        }

        public override bool Enabled
        {
            get { return MrCMSApplication.Get<PaymentSettings>().CashOnDeliveryEnabled; }
        }

        public override bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}