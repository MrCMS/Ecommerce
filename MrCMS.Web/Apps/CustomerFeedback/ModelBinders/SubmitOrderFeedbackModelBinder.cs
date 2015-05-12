using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.CustomerFeedback.ModelBinders
{
    public class SubmitOrderFeedbackModelBinder : MrCMSDefaultModelBinder
    {
        public SubmitOrderFeedbackModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;
            var ratings = form.AllKeys.Where(s => s.StartsWith("Rating-"));
            var recordsToUpdate = new List<FeedbackFacetRecordModel>();

            foreach (var rating in ratings)
            {
                int feedbackFacetRecordId;
                if (int.TryParse(Regex.Match(rating, @"\d+").Value, out feedbackFacetRecordId))
                {
                    var rate = form["Rating-" + feedbackFacetRecordId];
                    var msg = form["Message-" + feedbackFacetRecordId];

                    int i;
                    if (int.TryParse(rate, out i))
                    {
                        recordsToUpdate.Add(new FeedbackFacetRecordModel
                        {
                            Id = feedbackFacetRecordId,
                            Rating = i,
                            Message = msg
                        });
                    }
                }
            }

            Guid guid;
            Guid.TryParse(form["OrderGuid"], out guid);

            return new OrderFeedbackPostModel {Guid = guid, Records = recordsToUpdate};

            //return recordsToUpdate;
        }
    }
}