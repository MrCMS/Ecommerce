using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay.Results
{
    /// <summary>
    /// Action Result returned when an error occurs.
    /// </summary>
    public class ErrorResult : SagePayResult
    {
        public ErrorResult()
            : base(null)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.Output.WriteLine("Status=ERROR");
            context.HttpContext.Response.Output.WriteLine("RedirectURL={0}", BuildFailedUrl());
            context.HttpContext.Response.Output.WriteLine("StatusDetail=An error occurred when processing the request.");
        }
    }
}