using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    /// <summary>
    /// URL Resolution service
    /// </summary>
    public interface ISagePayUrlResolver
    {
        /// <summary>
        /// Resolves the Failed Transaction URL to be sent to SagePay when the payment fails.
        /// </summary>
        string BuildFailedTransactionUrl(string vendorTxCode);

        /// <summary>
        /// Resolves the Successful Transaction URL to be sent to SagePay when the payment succeeds.
        /// </summary>
        string BuildSuccessfulTransactionUrl(string vendorTxCode);

        /// <summary>
        /// Builds the notification URL.
        /// </summary>
        string BuildNotificationUrl();
    }

    public class MrCMSSagePayUrlResolver : ISagePayUrlResolver
    {
        private readonly SagePaySettings _sagePaySettings;

        public MrCMSSagePayUrlResolver(SagePaySettings sagePaySettings)
        {
            _sagePaySettings = sagePaySettings;
        }

        public string BuildFailedTransactionUrl(string vendorTxCode)
        {
            var schemeAndAuthority = GetSchemeAndAuthority();
            return string.Format("{0}/Apps/Ecommerce/SagePay/Failed/{1}", schemeAndAuthority, vendorTxCode);
        }

        public string BuildSuccessfulTransactionUrl(string vendorTxCode)
        {
            var schemeAndAuthority = GetSchemeAndAuthority();
            return string.Format("{0}/Apps/Ecommerce/SagePay/Success/{1}", schemeAndAuthority, vendorTxCode);
        }

        public string BuildNotificationUrl()
        {
            var schemeAndAuthority = GetSchemeAndAuthority();
            return string.Format("{0}/Apps/Ecommerce/SagePay/Notification", schemeAndAuthority);
        }

        private string GetSchemeAndAuthority()
        {
            var scheme = _sagePaySettings.RequiresSSL ? "https://" : "http://";
            var authority = _sagePaySettings.Site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');
            return string.Format("{0}{1}", scheme, authority);
        }
    }
}