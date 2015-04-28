using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters
{
    public class AddSuccessMessageAttribute : ActionFilterAttribute
    {
        private readonly string _message;

        public AddSuccessMessageAttribute(string message)
        {
            _message = message;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Controller.TempData.SuccessMessages().Add(_message);
        }
        
    }
}