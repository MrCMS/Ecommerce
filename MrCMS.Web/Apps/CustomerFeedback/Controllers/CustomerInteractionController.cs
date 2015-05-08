using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Pages;
using MrCMS.Web.Apps.CustomerFeedback.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Controllers
{
    public class CustomerInteractionController : MrCMSAppUIController<CustomerFeedbackApp>
    {
        private readonly IGetCustomerInteraction _getCustomerInteraction;

        public CustomerInteractionController(IGetCustomerInteraction getCustomerInteraction)
        {
            _getCustomerInteraction = getCustomerInteraction;
        }

        public ActionResult Show(CustomerInteraction page, [IoCModelBinder(typeof(OrderByGuidModelBinder))] Order order)
        {
            if (order == null)
                return Redirect("~/");

            ViewData["order"] = order;
            ViewData["interaction"] = _getCustomerInteraction.Get(order);

            return View(page);
        }
    }
}