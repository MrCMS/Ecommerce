using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class NewsletterTemplateOverride : IAutoMappingOverride<NewsletterTemplate>
    {
        public void Override(AutoMapping<NewsletterTemplate> mapping)
        {
            mapping.Map(template => template.BaseTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.Divider).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.FreeTextTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.ImageAndTextTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.ProductGridTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.ProductRowTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.ProductTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(template => template.BannerTemplate).CustomType<VarcharMax>().Length(4001);
        }
    }
}