using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.CustomerFeedback.Models;
using MrCMS.Web.Apps.CustomerFeedback.Pages;
using MrCMS.Web.Apps.CustomerFeedback.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Controllers
{
    public class CustomerInteractionController : MrCMSAppUIController<CustomerFeedbackApp>
    {
        private readonly IGetCustomerInteraction _getCustomerInteraction;
        private readonly IUniquePageService _uniquePageService;
        private readonly ICustomerInteractionService _customerInteractionService;

        public CustomerInteractionController(IGetCustomerInteraction getCustomerInteraction, 
            IUniquePageService uniquePageService, ICustomerInteractionService customerInteractionService)
        {
            _getCustomerInteraction = getCustomerInteraction;
            _uniquePageService = uniquePageService;
            _customerInteractionService = customerInteractionService;
        }

        public ActionResult Show(CustomerInteraction page, [IoCModelBinder(typeof(OrderByGuidModelBinder))] Order order)
        {
            if (order == null)
                return Redirect("~/");

            ViewData["order"] = order;
            ViewData["interaction"] = _getCustomerInteraction.Get(order);

            return View(page);
        }

        [HttpGet]
        public ActionResult Form(Order order)
        {
            return PartialView(new CustomerInteractionPostModel {Order = order});
        }


        [HttpPost]
        public ActionResult Submit(CustomerInteractionPostModel model)
        {
            _customerInteractionService.Add(model);
            TempData.SuccessMessages().Add("Message successfully submitted");
            return _uniquePageService.RedirectTo<CustomerInteraction>(new {id = model.Order.Guid});
        }

    }
}