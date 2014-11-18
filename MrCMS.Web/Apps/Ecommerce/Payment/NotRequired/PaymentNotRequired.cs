using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.NotRequired
{
    public class PaymentNotRequired : BasePaymentMethod
    {
        public PaymentNotRequired()
        {
            IsPaymentNotRequired = true;
        }

        public override string Name
        {
            get { return "Payment Not Required"; }
        }

        public override string SystemName
        {
            get { return "PaymentNotRequired"; }
        }

        public override string ControllerName
        {
            get { return "PaymentNotRequired"; }
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
            get { return true; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return false;
        }
    }
}