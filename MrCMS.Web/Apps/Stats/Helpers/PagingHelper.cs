using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Stats.Helpers
{
    public static class PagingHelper
    {
        public static IPagedList<TResult> Paged<TQuery, TResult>(this IQueryOver<TQuery, TQuery> queryBase, IProjection countQuery, int pageNumber, int? pageSize = null, bool enableCache = true)
            where TQuery : SystemEntity
        {
            var size = pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize;
            var rowCountQuery = queryBase.Clone();

            IEnumerable<TResult> results = queryBase.Skip((pageNumber - 1) * size).Take(size).MakeCacheable(enableCache).Future<TResult>();

            int rowCount = rowCountQuery.Select(countQuery).ClearOrders().MakeCacheable(enableCache).FutureValue<int>().Value;

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static IQueryOver<TQuery> MakeCacheable<TQuery>(this IQueryOver<TQuery> query, bool enableCache = true)
        {
            return enableCache ? query.Cacheable() : query;
        }
    }
}