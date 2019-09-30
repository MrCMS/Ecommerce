using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon
{
    public class ElavonPaymentMethod : BasePaymentMethod
    {
        private readonly ElavonSettings _elavonSettings;

        public ElavonPaymentMethod(ElavonSettings elavonSettings)
        {
            _elavonSettings = elavonSettings;
        }

        public override string Name
        {
            get { return "Elavon"; }
        }

        public override string SystemName
        {
            get { return "Elavon"; }
        }

        public override string ControllerName
        {
            get { return "Elavon"; }
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
            get { return _elavonSettings.Enabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}