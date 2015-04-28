using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class FreeTextDataOverride : IAutoMappingOverride<FreeTextTemplateData>
    {
        public void Override(AutoMapping<FreeTextTemplateData> mapping)
        {
            mapping.Map(data => data.FreeTextTemplate).MakeVarCharMax();
        }
    }
}