using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductOverride : IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.Map(product => product.Abstract).Length(500);
            mapping.HasManyToMany(product => product.Categories)
                   .Table("ProductCategories")
                   .AsList(part => part.Column("DisplayOrder"))
                   .Not.Inverse();
            mapping.HasManyToMany(product => product.AttributeOptions).Table("ProductAttributes").Not.Inverse();
        }
    }
    public class ProductVariantOverride : IAutoMappingOverride<ProductVariant>
    {
        public void Override(AutoMapping<ProductVariant> mapping)
        {
            mapping.HasMany(variant => variant.AttributeValues).KeyColumn("ProductVariantId").Cascade.All();
        }
    }
}