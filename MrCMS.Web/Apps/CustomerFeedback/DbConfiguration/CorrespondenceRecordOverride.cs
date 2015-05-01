using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.DbConfiguration
{
    public class CorrespondenceRecordOverride : IAutoMappingOverride<CorrespondenceRecord>
    {
        public void Override(AutoMapping<CorrespondenceRecord> mapping)
        {
            mapping.Map(x => x.MessageInfo).MakeVarCharMax();
        }
    }
}