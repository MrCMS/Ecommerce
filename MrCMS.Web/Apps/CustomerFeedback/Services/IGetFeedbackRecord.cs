using System;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public interface IGetFeedbackRecord
    {
        FeedbackRecord GetByOrderGuid(Guid guid);
    }
}