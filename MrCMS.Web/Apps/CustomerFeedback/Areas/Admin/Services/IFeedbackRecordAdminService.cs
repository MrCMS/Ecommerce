using MrCMS.Paging;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Controllers;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public interface IFeedbackRecordAdminService
    {
        IPagedList<FeedbackRecord> Search(FeedbackRecordAdminQuery query);
    }
}