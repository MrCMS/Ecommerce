using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductOptionOverride : IAutoMappingOverride<ProductOption>
    {
        public void Override(AutoMapping<ProductOption> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductOption_Name");
            mapping.HasManyToMany(option => option.Products).Table("Ecommerce_ProductOptions").Inverse();
        }
    }
}