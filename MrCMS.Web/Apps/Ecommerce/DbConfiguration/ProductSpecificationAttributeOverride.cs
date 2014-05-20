using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductSpecificationAttributeOverride : IAutoMappingOverride<ProductSpecificationAttribute>
    {
        public void Override(AutoMapping<ProductSpecificationAttribute> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductSpecificationAttribute_Name");
            mapping.Map(x => x.HideInSearch).Not.Nullable().Default("0");
            mapping.HasManyToMany(attribute => attribute.HiddenInCategories).Table("Ecommerce_CategoryHiddenSearchSpecifications").Inverse();
        }
    }
}