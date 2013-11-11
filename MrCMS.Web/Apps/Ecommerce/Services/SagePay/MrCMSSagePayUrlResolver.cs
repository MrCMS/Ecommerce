using System.Web.Mvc;
using System.Web.Routing;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class MrCMSSagePayUrlResolver : IUrlResolver
    {
        public string BuildFailedTransactionUrl(RequestContext context, string vendorTxCode)
        {
            return new UrlHelper(context).RouteUrl(null, new
                                                             {
                                                                 controller = "SagePay",
                                                                 action = "Failed",
                                                                 vendorTxCode
                                                             });
        }

        public string BuildSuccessfulTransactionUrl(RequestContext context, string vendorTxCode)
        {
            return new UrlHelper(context).RouteUrl(null, new
                                                             {
                                                                 controller = "SagePay",
                                                                 action = "Success",
                                                                 vendorTxCode
                                                             });
        }

        public string BuildNotificationUrl(RequestContext context)
        {
            return new UrlHelper(context).RouteUrl(null, new
                                                             {
                                                                 controller = "SagePay",
                                                                 action = "Notification",
                                                             });
        }
    }
}