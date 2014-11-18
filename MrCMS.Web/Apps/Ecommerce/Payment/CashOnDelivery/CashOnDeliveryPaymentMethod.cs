using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery
{
    public class CashOnDeliveryPaymentMethod : BasePaymentMethod
    {
        private readonly PaymentSettings _paymentSettings;

        public CashOnDeliveryPaymentMethod(PaymentSettings paymentSettings)
        {
            _paymentSettings = paymentSettings;
        }

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
            get { return _paymentSettings.CashOnDeliveryEnabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}