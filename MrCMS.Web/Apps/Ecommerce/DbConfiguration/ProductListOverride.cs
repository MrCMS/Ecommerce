using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductListOverride : IAutoMappingOverride<ProductList>
    {
        public void Override(AutoMapping<ProductList> mapping)
        {
            mapping.Map(x => x.Products).CustomType<VarcharMax>().Length(4001);
        }
    }
}