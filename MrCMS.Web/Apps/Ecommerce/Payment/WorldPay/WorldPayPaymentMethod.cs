using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay
{
    public class WorldPayPaymentMethod : BasePaymentMethod
    {
        private readonly WorldPaySettings _worldPaySettings;

        public WorldPayPaymentMethod(WorldPaySettings worldPaySettings)
        {
            _worldPaySettings = worldPaySettings;
        }

        public override string Name
        {
            get { return "WorldPay"; }
        }

        public override string SystemName
        {
            get { return "WorldPay"; }
        }

        public override string ControllerName
        {
            get { return "WorldPay"; }
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
            get { return _worldPaySettings.Enabled; }
        }

        protected override bool CanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}