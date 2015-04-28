using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class NewsletterTemplateOverride : IAutoMappingOverride<NewsletterTemplate>
    {
        public void Override(AutoMapping<NewsletterTemplate> mapping)
        {
            mapping.Map(template => template.BaseTemplate).MakeVarCharMax();
            mapping.Map(template => template.Divider).MakeVarCharMax();
            //mapping.HasMany(template => template.ContentItemTemplateDatas).KeyColumn("NewsletterTemplateId");
            //mapping.Map(template => template.FreeTextTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.ImageRightAndTextLeftTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.ImageLeftAndTextRightTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.ProductGridTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.ProductRowTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.ProductTemplate).CustomType<VarcharMax>().Length(4001);
            //mapping.Map(template => template.BannerTemplate).CustomType<VarcharMax>().Length(4001);
        }
    }
}