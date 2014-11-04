using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery
{
    public class CashOnDeliveryPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "Cash On Delivery"; }
        }

        public string SystemName
        {
            get { return "CashOnDelivery"; }
        }

        public string ControllerName
        {
            get { return "CashOnDelivery"; }
        }

        public string ActionName
        {
            get { return "Form"; }
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.ServiceBased; }
        }

        public bool Enabled
        {
            get { return MrCMSApplication.Get<PaymentSettings>().CashOnDeliveryEnabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}