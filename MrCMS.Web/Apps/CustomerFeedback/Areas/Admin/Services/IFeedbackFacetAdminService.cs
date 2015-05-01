using MrCMS.Paging;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public interface IFeedbackFacetAdminService
    {
        IPagedList<FeedbackFacet> Search(FeedbackFacetAdminSearchQuery query); 
        void Add(FeedbackFacet feedbackFacet);
        void Delete(FeedbackFacet feedbackFacet);
        void Edit(FeedbackFacet feedbackFacet);
    }
}