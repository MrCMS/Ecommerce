using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Stripe
{
    public class StripePaymentMethod : BasePaymentMethod
    {
        private readonly StripeSettings _settings;

        public StripePaymentMethod(StripeSettings settings)
        {
            _settings = settings;
        }

        public override string Name => "Pay by card (Stripe)";

        public override string SystemName => "Stripe";

        public override string ControllerName => "Stripe";

        public override string ActionName => "Form";

        public override PaymentType PaymentType => PaymentType.Redirection;

        public override bool Enabled => _settings.Enabled;

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}