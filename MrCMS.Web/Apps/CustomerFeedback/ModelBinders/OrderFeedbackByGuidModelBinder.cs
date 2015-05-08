using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.CustomerFeedback.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.CustomerFeedback.ModelBinders
{
    public class OrderFeedbackByGuidModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IGetFeedbackRecord _getFeedbackRecord;

        public OrderFeedbackByGuidModelBinder(IKernel kernel, IGetFeedbackRecord getFeedbackRecord) : base(kernel)
        {
            _getFeedbackRecord = getFeedbackRecord;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Guid guid;

            return
                Guid.TryParse(
                    GetValueFromContext(controllerContext, "guid") ??
                    Convert.ToString(controllerContext.RouteData.Values["guid"]), out guid)
                    ? _getFeedbackRecord.GetByOrderGuid(guid)
                    : null;
        }
    }
}