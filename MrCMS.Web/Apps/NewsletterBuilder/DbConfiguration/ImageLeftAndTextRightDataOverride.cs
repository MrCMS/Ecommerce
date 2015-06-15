using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class ImageLeftAndTextRightDataOverride : IAutoMappingOverride<ImageLeftAndTextRightTemplateData>
    {
        public void Override(AutoMapping<ImageLeftAndTextRightTemplateData> mapping)
        {
            mapping.Map(data => data.ImageLeftAndTextRightTemplate).MakeVarCharMax();
        }
    }
}