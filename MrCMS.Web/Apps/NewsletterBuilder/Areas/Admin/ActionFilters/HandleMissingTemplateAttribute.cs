using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Web.Areas.Admin.Helpers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters
{
    public class HandleMissingTemplateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null || viewResult.Model != null)
                return;
            filterContext.Controller.TempData.ErrorMessages().Add("Could not find the requested template");
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"controller", "NewsletterTemplate"},
                {"action", "Index"}
            });
        }
    }
}