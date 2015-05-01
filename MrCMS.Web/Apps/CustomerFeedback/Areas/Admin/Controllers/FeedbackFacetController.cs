using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Controllers
{
    public class FeedbackFacetController : MrCMSAppAdminController<CustomerFeedbackApp>
    {
        private readonly IFeedbackFacetAdminService _adminService;

        public FeedbackFacetController(IFeedbackFacetAdminService adminService)
        {
            _adminService = adminService;
        }

        public ViewResult Index(FeedbackFacetAdminSearchQuery query)
        {
            ViewData["results"] = _adminService.Search(query);
            return View(query);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new FeedbackFacet());
        }

        [HttpPost]
        public RedirectToRouteResult Add(FeedbackFacet feedbackFacet)
        {
            _adminService.Add(feedbackFacet);
            TempData.SuccessMessages().Add("New feedback facet added");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(FeedbackFacet feedbackFacet)
        {
            return PartialView(feedbackFacet);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(FeedbackFacet feedbackFacet)
        {
            _adminService.Edit(feedbackFacet);
            TempData.SuccessMessages().Add("New feedback facet updated");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(FeedbackFacet feedbackFacet)
        {
            return PartialView(feedbackFacet);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(FeedbackFacet feedbackFacet)
        {
            _adminService.Delete(feedbackFacet);
            TempData.SuccessMessages().Add("Feedback facet deleted");
            return RedirectToAction("Index");
        }

    }
}