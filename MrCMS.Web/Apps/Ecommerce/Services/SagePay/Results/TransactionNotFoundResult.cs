using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results
{
    /// <summary>
    /// Action Result to be returned when the transaction with the specified VendorTxCode could not be found.
    /// </summary>
    public class TransactionNotFoundResult : SagePayResult
    {
        public TransactionNotFoundResult(string vendorTxCode)
            : base(vendorTxCode)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.Output.WriteLine("Status=INVALID");
            context.HttpContext.Response.Output.WriteLine("RedirectURL={0}", BuildFailedUrl());
            context.HttpContext.Response.Output.WriteLine("StatusDetail=Unable to find the transaction in our database.");
        }
    }
}