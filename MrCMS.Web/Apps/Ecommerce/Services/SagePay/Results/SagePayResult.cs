using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results
{
    /// <summary>
    /// Base class for sage pay ActionResults
    /// </summary>
    public abstract class SagePayResult : ActionResult
    {
        readonly string vendorTxCode;

        protected SagePayResult(string vendorTxCode)
        {
            this.vendorTxCode = vendorTxCode;
        }

        protected string BuildFailedUrl()
        {
            var resolver = MrCMSApplication.Get<ISagePayUrlResolver>();
            return resolver.BuildFailedTransactionUrl(vendorTxCode);
        }

        protected string BuildSuccessUrl()
        {
            var resolver = MrCMSApplication.Get<ISagePayUrlResolver>();
            return resolver.BuildSuccessfulTransactionUrl(vendorTxCode);
        }
    }
}