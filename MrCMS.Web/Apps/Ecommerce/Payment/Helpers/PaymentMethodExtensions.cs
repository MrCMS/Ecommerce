using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Helpers
{
    public static class PaymentMethodExtensions
    {
        public static string GetUrl(this BasePaymentMethod method, UrlHelper urlHelper)
        {
            return urlHelper.Action(method.ActionName, method.ControllerName);
        }
        public static bool UseStandardFlow(this BasePaymentMethod method)
        {
            return method.PaymentType == PaymentType.Redirection || method.PaymentType == PaymentType.ServiceBased; 
        }
    }
}