using MrCMS.Web.Apps.CustomerFeedback.Models;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public interface ICustomerInteractionService
    {
        void Add(CustomerInteractionPostModel model);
    }
}