using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ImageAndTextOverride : IAutoMappingOverride<ImageAndText>
    {
        public void Override(AutoMapping<ImageAndText> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}