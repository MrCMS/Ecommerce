using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree
{
    public class BraintreePaymentMethod : BasePaymentMethod
    {
        private readonly BraintreeSettings _settings;

        public BraintreePaymentMethod(BraintreeSettings settings)
        {
            _settings = settings;
        }

        public override string Name
        {
            get { return "Braintree"; }
        }

        public override string SystemName
        {
            get { return "Braintree"; }
        }

        public override string ControllerName
        {
            get { return "Braintree"; }
        }

        public override string ActionName
        {
            get { return "Form"; }
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.Redirection; }
        }

        public override bool Enabled
        {
            get { return _settings.Enabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}