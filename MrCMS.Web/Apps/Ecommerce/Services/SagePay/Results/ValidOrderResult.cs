using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results
{
    /// <summary>
    /// ActionResult to be returned for a valid order (irrespective of whether payment failed or succeeded)
    /// </summary>
    public class ValidOrderResult : SagePayResult
    {
        readonly SagePayResponse response;

        public ValidOrderResult(string vendorTxCode, SagePayResponse response)
            : base(vendorTxCode)
        {
            this.response = response;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";

            if (response.Status == ResponseType.Error)
            {
                context.HttpContext.Response.Output.WriteLine("Status=INVALID");
            }
            else
            {
                context.HttpContext.Response.Output.WriteLine("Status=OK");
            }

            if (response.WasTransactionSuccessful)
            {
                context.HttpContext.Response.Output.WriteLine("RedirectURL={0}", BuildSuccessUrl());
            }
            else
            {
                context.HttpContext.Response.Output.WriteLine("RedirectURL={0}", BuildFailedUrl());
            }
        }
    }
}