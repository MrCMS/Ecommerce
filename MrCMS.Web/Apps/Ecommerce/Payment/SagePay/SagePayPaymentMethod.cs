using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagePayPaymentMethod : IPaymentMethod
    {
        public string Name
        {
            get { return "SagePay"; }
        }

        public string SystemName
        {
            get { return "SagePay"; }
        }

        public string ControllerName
        {
            get { return "SagePay"; }
        }

        public string ActionName
        {
            get { return "Form"; }
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.ServiceBased; }
        }

        public bool Enabled
        {
            get { return MrCMSApplication.Get<SagePaySettings>().Enabled; }
        }

        public bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }
}