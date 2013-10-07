using System;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Analytics;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public class AmazonApiLogService : IAmazonApiLogService
    {
        private readonly ISession _session;

        public AmazonApiLogService(ISession session)
        {
            _session = session;
        }

        public AmazonApiLog Save(AmazonApiLog amazonApiUsage)
        {
            _session.Transact(session => session.SaveOrUpdate(amazonApiUsage));
            return amazonApiUsage;
        }

        public AmazonApiLog GetByTimeRange(AmazonApiSection? apiSection, string apiOperation, DateTime from, DateTime to)
        {
            return _session.QueryOver<AmazonApiLog>()
                       .Where(x => from >= x.CreatedOn && x.CreatedOn <= to
                           && x.ApiSection == apiSection
                           && x.ApiOperation == apiOperation
                           && x.Site == CurrentRequestData.CurrentSite).Cacheable()
                       .SingleOrDefault();
        }
    }
}