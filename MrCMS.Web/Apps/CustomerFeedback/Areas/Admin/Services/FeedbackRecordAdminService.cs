using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public class FeedbackRecordAdminService : IFeedbackRecordAdminService
    {
        private readonly ISession _session;

        public FeedbackRecordAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<FeedbackRecord> Search(FeedbackRecordAdminQuery query)
        {
            return
                _session.QueryOver<FeedbackRecord>()
                    .OrderBy(x => x.CreatedOn)
                    .Desc.Cacheable()
                    .List()
                    .ToPagedList(query.Page);
        }
    }
}