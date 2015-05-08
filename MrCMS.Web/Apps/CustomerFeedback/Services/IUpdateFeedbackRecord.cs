using System.Collections.Generic;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public interface IUpdateFeedbackRecord
    {
        void Update(List<FeedbackFacetRecordModel> records);
    }
}