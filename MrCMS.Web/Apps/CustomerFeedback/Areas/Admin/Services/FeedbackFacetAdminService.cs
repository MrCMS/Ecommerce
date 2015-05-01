using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public class FeedbackFacetAdminService : IFeedbackFacetAdminService
    {
        private readonly ISession _session;

        public FeedbackFacetAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<FeedbackFacet> Search(FeedbackFacetAdminSearchQuery query)
        {
            return
                _session.QueryOver<FeedbackFacet>()
                    .OrderBy(x => x.DisplayOrder)
                    .Asc.Cacheable()
                    .List()
                    .ToPagedList(query.Page);
        }

        public void Add(FeedbackFacet feedbackFacet)
        {
            _session.Transact(session => session.Save(feedbackFacet));
        }

        public void Delete(FeedbackFacet feedbackFacet)
        {
            _session.Transact(session => session.Delete(feedbackFacet));
        }

        public void Edit(FeedbackFacet feedbackFacet)
        {
            _session.Transact(session => session.Update(feedbackFacet));
        }
    }
}