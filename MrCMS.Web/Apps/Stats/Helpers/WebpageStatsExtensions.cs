using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Stats.Areas.Admin.Models;
using MrCMS.Web.Apps.Stats.Entities;
using MrCMS.Website;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Stats.Helpers
{
    public static class WebpageStatsExtensions
    {
        public static int TotalViewsFor(this HtmlHelper helper, Webpage webpage)
        {
            if (webpage == null)
                return 0;

            var session = helper.ViewContext.HttpContext.Get<ISession>();

            AnalyticsPageView pageView = null;
            AnalyticsSession analyticsSession = null;
            AnalyticsUser analyticsUser = null;
            IQueryOver<AnalyticsPageView, AnalyticsPageView> queryOver = session.QueryOver(() => pageView)
                .JoinAlias(() => pageView.AnalyticsSession, () => analyticsSession)
                .JoinAlias(() => analyticsSession.AnalyticsUser, () => analyticsUser)
                .Where(x => x.Webpage.Id == webpage.Id);


            PageViewResult result = null;
            var viewResult = queryOver
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
                .Take(1).SingleOrDefault<PageViewResult>();
            return viewResult == null ? 0 : viewResult.Total;
        }
    }
}