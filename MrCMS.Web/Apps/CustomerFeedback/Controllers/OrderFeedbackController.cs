using System.Collections.Generic;
using System.Web.Mvc;
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

        public OrderFeedbackController(IUpdateFeedbackRecord updateFeedbackRecord)
        {
            _updateFeedbackRecord = updateFeedbackRecord;
        }

        public ActionResult Show(OrderFeedback page, [IoCModelBinder(typeof(OrderFeedbackByGuidModelBinder))] FeedbackRecord record)
        {
            if (record == null)
                return Redirect("~/");

            ViewData["model"] = record;

            return View(page);
        }

        [HttpPost]
        public ActionResult Submit([IoCModelBinder(typeof(SubmitOrderFeedbackModelBinder))] List<FeedbackFacetRecordModel> records)
        {
            _updateFeedbackRecord.Update(records);
            return Redirect("~/");
        }
    }

    public class UpdateFeedbackModel
    {
        public UpdateFeedbackModel()
        {
            FacetRecords = new List<FeedbackFacetRecord>();
        }
        public int FeedbackRecordId { get; set; }
        public List<FeedbackFacetRecord> FacetRecords { get; set; } 
    }

    public class FeedbackFacetRecordModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
    }
}