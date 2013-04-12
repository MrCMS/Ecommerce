using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductOverride: IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.HasManyToMany(product => product.Categories).Table("ProductCategories").Not.Inverse();
            mapping.HasManyToMany(product => product.AttributeOptions).Table("ProductAttributes").Not.Inverse();
        }
    }
    public class ProductVariantOverride: IAutoMappingOverride<ProductVariant>
    {
        public void Override(AutoMapping<ProductVariant> mapping)
        {
            mapping.HasMany(variant => variant.AttributeValues).KeyColumn("ProductVariantId").Cascade.All();
        }
    }
}