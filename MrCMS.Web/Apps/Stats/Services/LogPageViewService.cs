using System;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Stats.Controllers;
using MrCMS.Web.Apps.Stats.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Stats.Services
{
    public class LogPageViewService : ILogPageViewService
    {
        private readonly ISession _session;
        private readonly IGetCurrentUser _getCurrentUser;

        public LogPageViewService(ISession session, IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
        }

        public void LogPageView(PageViewInfo info)
        {
            var analyticsUser = GetCurrentUser(info.User);
            var userIsNew = analyticsUser == null;
            if (userIsNew)
            {
                analyticsUser = new AnalyticsUser
                {
                    User = _getCurrentUser.Get(),
                    Guid = info.User
                };
            }
            var analyticsSession = GetCurrentSession(info.Session);
            var sessionIsNew = analyticsSession == null;
            if (sessionIsNew)
            {
                analyticsSession = new AnalyticsSession
                {
                    AnalyticsUser = analyticsUser,
                    Guid = info.Session
                };
            }

            var pageView = new AnalyticsPageView
            {
                Webpage = GetWebpage(info.Url),
                Url = info.Url,
                AnalyticsSession = analyticsSession,
            };
            _session.Transact(session =>
            {
                if (userIsNew)
                    session.Save(analyticsUser);
                if (sessionIsNew)
                    session.Save(analyticsSession);
                session.Save(pageView);
            });
        }

        private Webpage GetWebpage(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                return null;

            var path = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrWhiteSpace(path))
                return CurrentRequestData.HomePage;
            var page = _session.QueryOver<Webpage>()
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