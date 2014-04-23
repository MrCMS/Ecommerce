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
            var statelessSession = _sessionFactory.OpenStatelessSession();
            var sessionDatas =
                statelessSession.QueryOver<SessionData>().Where(data => data.ExpireOn <= CurrentRequestData.Now).List();

            using (var transaction = statelessSession.BeginTransaction())
            {
                foreach (var sessionData in sessionDatas)
                {
                    statelessSession.Delete(sessionData);
                }
                transaction.Commit();
            }
        }
    }
}