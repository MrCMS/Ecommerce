using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Search;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Search
{
    public class SearchLogService : ISearchLogService
    {
        private readonly ISession _session;

        public SearchLogService(ISession session)
        {
            _session = session;
        }

        public SearchLog GetById(int id)
        {
            return _session.QueryOver<SearchLog>().Where(x => x.Id == id).SingleOrDefault();
        }

        public IList<SearchLog> GetAll()
        {
            return _session.QueryOver<SearchLog>().OrderBy(x => x.CreatedOn).Desc.Cacheable().List();
        }

        public void Add(SearchLog searchLog)
        {
            _session.Transact(session => session.Save(searchLog));
        }

        public void Update(SearchLog searchLog)
        {
            _session.Transact(session => session.Update(searchLog));
        }

        public void Delete(SearchLog searchLog)
        {
            _session.Transact(session => session.Delete(searchLog));
        }

        public IPagedList<SearchLog> GetPaged(int pageNum, string search, int pageSize = 10)
        {
            return BaseQuery(search).Paged(pageNum, pageSize);
        }

        private IQueryOver<SearchLog, SearchLog> BaseQuery(string search)
        {
            if (String.IsNullOrWhiteSpace(search))
                return
                _session.QueryOver<SearchLog>()
                        .OrderBy(entry => entry.Text).Asc;
            return
                _session.QueryOver<SearchLog>().Where(x => x.Text.IsInsensitiveLike(search, MatchMode.Anywhere))
                        .OrderBy(entry => entry.Text).Asc;
        }
    }
}