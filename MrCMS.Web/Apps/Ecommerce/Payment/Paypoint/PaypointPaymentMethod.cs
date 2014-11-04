using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PaypointPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "Pay by card"; }
        }

        public string SystemName
        {
            get { return "Paypoint"; }
        }

        public string ControllerName
        {
            get { return "Paypoint"; }
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
            get { return MrCMSApplication.Get<PaypointSettings>().Enabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}