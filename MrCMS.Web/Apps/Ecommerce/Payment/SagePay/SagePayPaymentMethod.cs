using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePayPaymentMethod : BasePaymentMethod
    {
        public override string Name { get { return "SagePay"; } }
        public override string SystemName { get { return "SagePay"; } }

        public override string ControllerName
        {
            get { return "SagePay"; }
        }

        public override string ActionName
        {
            get { return "Form"; }
        }

        public override PaymentType PaymentType { get { return PaymentType.ServiceBased; } }
        public override bool Enabled { get { return MrCMSApplication.Get<SagePaySettings>().Enabled; } }
        public override bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}