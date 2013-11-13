using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Tasks
{
    public class ClearExpiredSessionDataTask : BackgroundTask
    {
        public ClearExpiredSessionDataTask(Site site)
            : base(site)
        {
        }

        public override void Execute()
        {
            var statelessSession = MrCMSApplication.Get<ISessionFactory>().OpenStatelessSession();
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