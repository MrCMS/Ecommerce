using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Stats.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Stats.Filters
{
    public class AnalyticsSessionFilter : ActionFilterAttribute
    {
        private const string AnalyticsUserKey = "mrcms.analytics.user";
        private const string AnalyticsSessionKey = "mrcms.analytics.session";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var botAgentsAndIPs = filterContext.HttpContext.Get<BotAgentsAndIPs>();
            if (botAgentsAndIPs.IsABot(filterContext.HttpContext.Request))
                return;

            var httpContext = filterContext.HttpContext;
            var now = CurrentRequestData.Now;
            httpContext.Response.SetCookie(
                new HttpCookie(AnalyticsUserKey, CurrentRequestData.UserGuid.ToString())
                {
                    Expires = now.AddYears(1)
                });
            var sessionCookie = httpContext.Request.Cookies[AnalyticsSessionKey];
            var sessionGuid = sessionCookie != null ? sessionCookie.Value : Guid.NewGuid().ToString();
            httpContext.Response.SetCookie(
                new HttpCookie(AnalyticsSessionKey, sessionGuid)
                {
                    Expires = now.AddMinutes(20)
                });
        }
    }
}