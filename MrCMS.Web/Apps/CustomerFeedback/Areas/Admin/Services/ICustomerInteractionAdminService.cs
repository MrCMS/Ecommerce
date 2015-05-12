using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public interface ICustomerInteractionAdminService
    {
        void Add(CorrespondenceRecord record);
    }
}