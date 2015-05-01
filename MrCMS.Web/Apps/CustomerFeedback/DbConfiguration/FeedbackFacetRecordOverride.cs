using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.DbConfiguration
{
    public class FeedbackFacetRecordOverride : IAutoMappingOverride<FeedbackFacetRecord>
    {
        public void Override(AutoMapping<FeedbackFacetRecord> mapping)
        {
            mapping.Map(x => x.Message).MakeVarCharMax();
        }
    }
}