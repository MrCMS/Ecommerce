using PayPal.Authentication;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalSecurityService
    {
        ICredential GetCredentials();
    }

    public class PayPalSecurityService : IPayPalSecurityService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalSecurityService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public ICredential GetCredentials()
        {
            return _payPalExpressCheckoutSettings.HaveBusinessAccount
                       ? new SignatureCredential(_payPalExpressCheckoutSettings.UserName,
                                                 _payPalExpressCheckoutSettings.Password,
                                                 _payPalExpressCheckoutSettings.Signature)
                       : new SignatureCredential(null, null, null,
                                                 new SubjectAuthorization(
                                                     _payPalExpressCheckoutSettings.SubjectEmailAddress));
        }
    }
}