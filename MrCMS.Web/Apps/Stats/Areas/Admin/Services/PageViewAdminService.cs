using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Stats.Areas.Admin.Models;
using MrCMS.Web.Apps.Stats.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Services
{
    public class PageViewAdminService : IPageViewAdminService
    {
        private readonly ISession _session;

        public PageViewAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<PageViewResult> Search(PageViewSearchQuery query)
        {
            AnalyticsPageView pageView = null;
            AnalyticsSession analyticsSession = null;
            AnalyticsUser analyticsUser = null;
            IQueryOver<AnalyticsPageView, AnalyticsPageView> queryOver = _session.QueryOver(() => pageView)
                .JoinAlias(() => pageView.AnalyticsSession, () => analyticsSession)
                .JoinAlias(() => analyticsSession.AnalyticsUser, () => analyticsUser);
            if (!string.IsNullOrWhiteSpace(query.Url))
            {
                queryOver = queryOver.Where(view => view.Url.IsInsensitiveLike(query.Url, MatchMode.Anywhere));
            }

            queryOver = queryOver.Where(() => pageView.CreatedOn >= query.From && pageView.CreatedOn <= query.To);
            
            switch (query.SearchType)
            {
                case PageViewSearchType.UsersOnly:
                    queryOver = queryOver.Where(() => analyticsUser.User != null);
                    break;
                case PageViewSearchType.AnonymousOnly:
                    queryOver = queryOver.Where(() => analyticsUser.User == null);
                    break;
            }

            PageViewResult result = null;
            IPagedList<PageViewResult> pageViewResults = queryOver
                .SelectList(builder =>
                    builder
                        .SelectGroup(() => pageView.Url)
                        .WithAlias(() => result.Url)
                        .SelectGroup(() => pageView.Webpage.Id)
                        .WithAlias(() => result.WebpageId)
                        .SelectCountDistinct(() => analyticsUser.Id)
                        .WithAlias(() => result.Unique)
                        .SelectCountDistinct(() => analyticsSession.Id)
                        .WithAlias(() => result.Sessions)
                        .SelectCountDistinct(() => pageView.Id)
                        .WithAlias(() => result.Total)
                )
                .TransformUsing(Transformers.AliasToBean<PageViewResult>())
                .OrderBy(Projections.CountDistinct(() => analyticsUser.Id)).Desc
                .ThenBy(Projections.CountDistinct(() => analyticsSession.Id)).Desc
                .ThenBy(Projections.CountDistinct(() => pageView.Id)).Desc
                .Paged<AnalyticsPageView, PageViewResult>(Projections.CountDistinct(() => pageView.Url), query.Page, enableCache:false); //todo enable cache when Nhibernate is updated to 4.1

            List<int?> ids = pageViewResults.Select(viewResult => viewResult.WebpageId).Where(i => i.HasValue).ToList();
            Dictionary<int, Webpage> webpages =
                _session.QueryOver<Webpage>()
                    .Where(webpage => webpage.Id.IsIn(ids))
                    .Cacheable()
                    .List()
                    .ToDictionary(webpage => webpage.Id, webpage => webpage);
            foreach (PageViewResult pageViewResult in pageViewResults)
            {
                if (!pageViewResult.WebpageId.HasValue)
                    continue;
                if (!webpages.ContainsKey(pageViewResult.WebpageId.Value))
                    continue;
                pageViewResult.WebpageName = webpages[pageViewResult.WebpageId.Value].Name;
            }

            return pageViewResults;
        }

        public List<SelectListItem> GetSearchTypeOptions()
        {
            return Enum.GetValues(typeof(PageViewSearchType)).Cast<PageViewSearchType>()
                .BuildSelectItemList(type => type.ToString(), emptyItem: null);
        }
    }
}