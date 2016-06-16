using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Filters
{
    public class MustBeLoggedInAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
            {
                var uniquePageService = filterContext.HttpContext.Get<IUniquePageService>();
                filterContext.Result = uniquePageService.RedirectTo<LoginPage>();
            }
        }
    }
}