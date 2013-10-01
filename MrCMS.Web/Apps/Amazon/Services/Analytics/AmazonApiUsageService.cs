using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Analytics
{
    public class AmazonApiUsageService : IAmazonApiUsageService
    {
        private readonly ISession _session;

        public AmazonApiUsageService(ISession session)
        {
            _session = session;
        }

        public AmazonApiUsage Save(AmazonApiUsage amazonApiUsage)
        {
            _session.Transact(session => session.SaveOrUpdate(amazonApiUsage));
            return amazonApiUsage;
        }

        public AmazonApiUsage GetForToday(AmazonApiSection? apiSection, string apiOperation)
        {
            return _session.QueryOver<AmazonApiUsage>()
                       .Where(x => x.Day == CurrentRequestData.Now.Date
                           && x.ApiSection == apiSection
                           && x.ApiOperation == apiOperation
                           && x.Site == CurrentRequestData.CurrentSite).Cacheable()
                       .SingleOrDefault();
        }
    }
}