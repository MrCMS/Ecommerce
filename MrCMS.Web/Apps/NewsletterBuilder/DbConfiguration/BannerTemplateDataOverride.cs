using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class BannerTemplateDataOverride : IAutoMappingOverride<BannerTemplateData>
    {
        public void Override(AutoMapping<BannerTemplateData> mapping)
        {
            mapping.Map(data => data.BannerTemplate).MakeVarCharMax();
        }
    }
}