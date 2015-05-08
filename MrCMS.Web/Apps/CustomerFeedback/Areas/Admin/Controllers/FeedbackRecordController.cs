using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Controllers
{
    public class FeedbackRecordController : MrCMSAppAdminController<CustomerFeedbackApp>
    {
        private readonly IFeedbackRecordAdminService _service;

        public FeedbackRecordController(IFeedbackRecordAdminService service)
        {
            _service = service;
        }

        public ViewResult Index(FeedbackRecordAdminQuery query)
        {
            ViewData["results"] = _service.Search(query);
            return View(query);
        }

        public ViewResult Show(FeedbackRecord record)
        {
            return View(record);
        }
    }
}