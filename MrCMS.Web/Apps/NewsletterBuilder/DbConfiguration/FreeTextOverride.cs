using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class FreeTextOverride : IAutoMappingOverride<FreeText>
    {
        public void Override(AutoMapping<FreeText> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}