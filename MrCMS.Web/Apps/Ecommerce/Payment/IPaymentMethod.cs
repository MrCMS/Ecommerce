using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment
{
    public abstract class BasePaymentMethod
    {
        public abstract string Name { get; }
        public abstract string SystemName { get; }
        public abstract string ControllerName { get; }
        public abstract string ActionName { get; }
        public abstract PaymentType PaymentType { get; }
        public abstract bool Enabled { get; }
        protected abstract bool StandardCanUseLogic(CartModel cart);

        public bool CanUse(CartModel cart)
        {
            var anythingToPay = cart.AnythingToPay();
            if (anythingToPay)
            {
                return StandardCanUseLogic(cart);
            }
            return IsPaymentNotRequired;
        }

        public bool IsPaymentNotRequired { get; protected set; }
    }
}