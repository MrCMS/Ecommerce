using System.Web.Mvc;
using MrCMS.Web.Apps.Stats.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Stats.Filters
{
    public class PreventBotsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var botAgentsAndIPs = filterContext.HttpContext.Get<BotAgentsAndIPs>();
            if (botAgentsAndIPs.IsABot(filterContext.HttpContext.Request))
                filterContext.Result = new EmptyResult();
        }
    }
}