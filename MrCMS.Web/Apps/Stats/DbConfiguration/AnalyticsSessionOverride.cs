using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Stats.Entities;

namespace MrCMS.Web.Apps.Stats.DbConfiguration
{
    public class AnalyticsSessionOverride :IAutoMappingOverride<AnalyticsSession>
    {
        public void Override(AutoMapping<AnalyticsSession> mapping)
        {
            mapping.Map(session => session.UserAgent).MakeVarCharMax();
        }
    }
}