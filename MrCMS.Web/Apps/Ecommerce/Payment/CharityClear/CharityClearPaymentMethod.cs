using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear
{
    public class CharityClearPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "Charity Clear"; }
        }

        public string SystemName
        {
            get { return "CharityClear"; }
        }

        public string ControllerName
        {
            get { return "CharityClear"; }
        }

        public string ActionName
        {
            get { return "Form"; }
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.Redirection; }
        }

        public bool Enabled
        {
            get { return MrCMSApplication.Get<CharityClearSettings>().Enabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}