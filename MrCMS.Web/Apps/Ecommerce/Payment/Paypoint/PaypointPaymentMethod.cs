using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PaypointPaymentMethod : BasePaymentMethod
    {
        public override string Name
        {
            get { return "Pay by card"; }
        }

        public override string SystemName
        {
            get { return "Paypoint"; }
        }

        public override string ControllerName
        {
            get { return "Paypoint"; }
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
            get { return MrCMSApplication.Get<PaypointSettings>().Enabled; }
        }

        public override bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}