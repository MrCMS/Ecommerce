using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePayPaymentMethod : BasePaymentMethod
    {
        private readonly SagePaySettings _sagePaySettings;

        public SagePayPaymentMethod(SagePaySettings sagePaySettings)
        {
            _sagePaySettings = sagePaySettings;
        }

        public override string Name
        {
            get { return "SagePay"; }
        }

        public override string SystemName
        {
            get { return "SagePay"; }
        }

        public override string ControllerName
        {
            get { return "SagePay"; }
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
            get { return _sagePaySettings.Enabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}