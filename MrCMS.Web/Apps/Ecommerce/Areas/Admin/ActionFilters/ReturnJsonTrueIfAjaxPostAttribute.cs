using System.Web.Mvc;
using MrCMS.Website.ActionResults;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ActionFilters
{
    public class ReturnJsonTrueIfAjaxPostAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception==null)
            {
                filterContext.Result = new JsonNetResult {Data = true};
            }
        }
    }
}