using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class UrlHistoryService : IUrlHistoryService
    {
        private readonly ISession _session;

        public UrlHistoryService (ISession session)
        {
            _session = session;
        }

        public void Delete(UrlHistory urlHistory)
        {
            _session.Transact(session => _session.Delete(urlHistory));
        }

        public void Add(UrlHistory urlHistory)
        {
            _session.Transact(session => session.Save(urlHistory));
        }

        public UrlHistory GetByUrlSegment(string urlSegment)
        {
            return _session.QueryOver<UrlHistory>().Where(x => x.UrlSegment.IsLike(urlSegment, MatchMode.Exact)).Cacheable(). SingleOrDefault();
        }
    }
}