using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class ImageRightAndTextLeftDataOverride : IAutoMappingOverride<ImageRightAndTextLeftTemplateData>
    {
        public void Override(AutoMapping<ImageRightAndTextLeftTemplateData> mapping)
        {
            mapping.Map(data => data.ImageRightAndTextLeftTemplate).MakeVarCharMax();
        }
    }
}