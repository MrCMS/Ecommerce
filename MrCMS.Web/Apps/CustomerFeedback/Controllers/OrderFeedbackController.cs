using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.ModelBinders;
using MrCMS.Web.Apps.CustomerFeedback.Pages;
using MrCMS.Web.Apps.CustomerFeedback.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Controllers
{
    public class OrderFeedbackController : MrCMSAppUIController<CustomerFeedbackApp>
    {
        private readonly IUpdateFeedbackRecord _updateFeedbackRecord;
        private readonly IUniquePageService _uniquePageService;

        public OrderFeedbackController(IUpdateFeedbackRecord updateFeedbackRecord, IUniquePageService uniquePageService)
        {
            _updateFeedbackRecord = updateFeedbackRecord;
            _uniquePageService = uniquePageService;
        }

        public ActionResult Show(OrderFeedback page, [IoCModelBinder(typeof(OrderFeedbackByGuidModelBinder))] FeedbackRecord record)
        {
            if (record == null)
                return Redirect("~/");

            ViewData["model"] = record;

            return View(page);
        }

        [HttpPost]
        public ActionResult Submit([IoCModelBinder(typeof(SubmitOrderFeedbackModelBinder))] OrderFeedbackPostModel model)
        {
            _updateFeedbackRecord.Update(model.Records);
            return _uniquePageService.RedirectTo<OrderFeedback>(new { guid = model.Guid });
        }
    }

    public class OrderFeedbackPostModel
    {
        public OrderFeedbackPostModel()
        {
            Records = new List<FeedbackFacetRecordModel>();
        }
        public Guid Guid { get; set; }
        public List<FeedbackFacetRecordModel> Records { get; set; }
    }

    public class FeedbackFacetRecordModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
    }
}