using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Tasks
{
    public class ClearExpiredSessionDataTask : SchedulableTask
    {
        private readonly ISessionFactory _sessionFactory;

        public ClearExpiredSessionDataTask(Site site, ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public override int Priority
        {
            get { return 100; }
        }

        protected override void OnExecute()
        {
            IStatelessSession statelessSession = _sessionFactory.OpenStatelessSession();
            IList<SessionData> sessionDatas =
                statelessSession.QueryOver<SessionData>()
                    .Where(data => data.ExpireOn <= CurrentRequestData.Now || data.CreatedOn < CurrentRequestData.Now.AddDays(-90)).List();

            using (ITransaction transaction = statelessSession.BeginTransaction())
            {
                foreach (SessionData sessionData in sessionDatas)
                {
                    statelessSession.Delete(sessionData);
                }
                transaction.Commit();
            }
        }
    }
}