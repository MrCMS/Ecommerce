using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Controllers
{
    public class OrderFeedbackController : MrCMSAppUIController<CustomerFeedbackApp>
    {
        public ViewResult Show(OrderFeedback page)
        {
            return View(page);
        }
    }
}