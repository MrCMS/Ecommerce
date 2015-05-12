using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.DbConfiguration
{
    public class FeedbackOverride : IAutoMappingOverride<Feedback>
    {
        public void Override(AutoMapping<Feedback> mapping)
        {
            mapping.Map(x => x.Message).MakeVarCharMax();
        }
    }
}