using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear
{
    public class CharityClearPaymentMethod : BasePaymentMethod
    {
        private readonly CharityClearSettings _charityClearSettings;

        public CharityClearPaymentMethod(CharityClearSettings charityClearSettings)
        {
            _charityClearSettings = charityClearSettings;
        }

        public override string Name
        {
            get { return "Charity Clear"; }
        }

        public override string SystemName
        {
            get { return "CharityClear"; }
        }

        public override string ControllerName
        {
            get { return "CharityClear"; }
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
            get { return _charityClearSettings.Enabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}