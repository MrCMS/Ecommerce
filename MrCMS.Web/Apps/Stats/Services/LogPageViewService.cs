using System;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Stats.Entities;
using MrCMS.Web.Apps.Stats.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Stats.Services
{
    public class LogPageViewService : ILogPageViewService
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly HttpRequestBase _request;
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public LogPageViewService(IStatelessSession session, IGetCurrentUser getCurrentUser, HttpRequestBase request,
            Site site)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
            _request = request;
            _site = site;
        }

        public void LogPageView(PageViewInfo info)
        {
            User user = _getCurrentUser.Get();
            var site = _session.Get<Site>(_site.Id);
            DateTime now = CurrentRequestData.Now;
            AnalyticsUser analyticsUser = GetCurrentUser(user == null ? info.User : user.Guid);
            bool userIsNew = analyticsUser == null;
            if (userIsNew)
            {
                analyticsUser = new AnalyticsUser
                {
                    User = user,
                    CreatedOn = now,
                    UpdatedOn = now,
                };
                analyticsUser.SetGuid(info.User);
                _session.Insert(analyticsUser);
            }
            AnalyticsSession analyticsSession = GetCurrentSession(info.Session);
            bool sessionIsNew = analyticsSession == null;
            if (sessionIsNew)
            {
                analyticsSession = new AnalyticsSession
                {
                    AnalyticsUser = analyticsUser,
                    IP = _request.GetCurrentIP(),
                    UserAgent = _request.UserAgent,
                    Site = site,
                    CreatedOn = now,
                    UpdatedOn = now,
                };
                analyticsSession.SetGuid(info.Session);
                _session.Insert(analyticsSession);
            }

            var pageView = new AnalyticsPageView
            {
                Webpage = GetWebpage(info.Url),
                Url = info.Url,
                AnalyticsSession = analyticsSession,
                Site = site,
                CreatedOn = now,
                UpdatedOn = now,
            };

            _session.Insert(pageView);
        }

        private Webpage GetWebpage(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                return null;

            string path = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrWhiteSpace(path))
                return CurrentRequestData.HomePage;
            Webpage page = _session.QueryOver<Webpage>()
                .Where(webpage => webpage.UrlSegment == path)
                .Cacheable()
                .List()
                .FirstOrDefault();
            return page;
        }

        private AnalyticsSession GetCurrentSession(Guid session)
        {
            return _session.QueryOver<AnalyticsSession>().Where(user => user.Guid == session)
                .Cacheable()
                .List().FirstOrDefault();
        }

        private AnalyticsUser GetCurrentUser(Guid guid)
        {
            return _session.QueryOver<AnalyticsUser>().Where(user => user.Guid == guid)
                .Cacheable()
                .List().FirstOrDefault();
        }
    }
}