using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results
{
    /// <summary>
    /// Action result used when an invalid signature is returned from SagePay.
    /// </summary>
    public class InvalidSignatureResult : SagePayResult
    {
        public InvalidSignatureResult(string vendorTxCode)
            : base(vendorTxCode)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.Output.WriteLine("Status=INVALID");
            context.HttpContext.Response.Output.WriteLine("RedirectURL={0}", BuildFailedUrl());
            context.HttpContext.Response.Output.WriteLine("StatusDetail=Cannot match the MD5 Hash. Order might be tampered with.");
        }
    }
}