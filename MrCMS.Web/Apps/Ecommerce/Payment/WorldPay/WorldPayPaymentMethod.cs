using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay
{
    public class WorldPayPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "WorldPay"; }
        }

        public string SystemName
        {
            get { return "WorldPay"; }
        }

        public string ControllerName
        {
            get { return "WorldPay"; }
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
            get { return MrCMSApplication.Get<WorldPaySettings>().Enabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}