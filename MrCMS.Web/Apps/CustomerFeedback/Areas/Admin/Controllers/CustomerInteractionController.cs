using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Controllers
{
    public class CustomerInteractionController : MrCMSAppAdminController<CustomerFeedbackApp>
    {
        private readonly IGetCustomerInteraction _getCustomerInteraction;
        private readonly ICustomerInteractionAdminService _adminService;


        public CustomerInteractionController(IGetCustomerInteraction getCustomerInteraction, ICustomerInteractionAdminService adminService)
        {
            _getCustomerInteraction = getCustomerInteraction;
            _adminService = adminService;
        }

        public ViewResult ShowInteraction(Order order)
        {
            ViewData["data"] = _getCustomerInteraction.Get(order);
            return View(order);
        }

        public PartialViewResult Add(Order order)
        {
            var model = new CorrespondenceRecord
            {
                Order = order,
                CorrespondenceDirection = CorrespondenceDirection.Outgoing,
                User = CurrentRequestData.CurrentUser
            };
            return PartialView(model);
        }

        [HttpPost]
        public RedirectToRouteResult Add(CorrespondenceRecord record)
        {
            _adminService.Add(record);
            TempData.SuccessMessages().Add("Record added.");
            return RedirectToAction("ShowInteraction", new { id = record.Order.Id });
        }
    }
}