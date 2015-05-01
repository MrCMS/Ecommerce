using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Controllers
{
    public class CustomerInteractionController : MrCMSAppUIController<CustomerFeedbackApp>
    {
        public ViewResult Show(CustomerInteraction page)
        {
            return View(page);
        }
    }
}