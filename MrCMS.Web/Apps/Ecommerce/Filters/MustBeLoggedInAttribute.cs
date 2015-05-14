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
            var uniquePageService = filterContext.HttpContext.Get<IUniquePageService>();
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                filterContext.Result = uniquePageService.RedirectTo<LoginPage>();
        }
    }
}