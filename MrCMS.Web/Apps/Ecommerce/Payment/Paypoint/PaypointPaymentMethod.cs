using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PaypointPaymentMethod : BasePaymentMethod
    {
        private readonly PaypointSettings _paypointSettings;

        public PaypointPaymentMethod(PaypointSettings paypointSettings)
        {
            _paypointSettings = paypointSettings;
        }

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
            get { return _paypointSettings.Enabled; }
        }

        protected override bool CanUseLogic(CartModel cart)
        {
            return cart.IsPayPalTransaction;
        }
    }
}