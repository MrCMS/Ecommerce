using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductVariantOverride : IAutoMappingOverride<ProductVariant>
    {
        public void Override(AutoMapping<ProductVariant> mapping)
        {
            mapping.HasMany(variant => variant.OptionValues).KeyColumn("ProductVariantId").Cascade.All();
            mapping.HasOne(variant => variant.GoogleBaseProduct)
                   .PropertyRef(product => product.ProductVariant)
                   .Cascade.All().Fetch.Join();
            mapping.HasMany(variant => variant.PriceBreaks).Cascade.All();

            mapping.Map(x => x.SKU).Index("IX_ProductVariant_SKU");

            mapping.HasManyToMany(product => product.ShippingMethods)
                .Table("Ecommerce_ProductVariantShippingMethods")
                .AsList(part => part.Column("DisplayOrder"))
                .Not.Inverse();
        }
    }

    public class ProductSpecificationAttributeOptionOverride : IAutoMappingOverride<ProductSpecificationAttributeOption>
    {
        public void Override(AutoMapping<ProductSpecificationAttributeOption> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductSpecificationAttributeOption_Name");
        }
    }

    public class ProductSpecificationAttributeOverride : IAutoMappingOverride<ProductSpecificationAttribute>
    {
        public void Override(AutoMapping<ProductSpecificationAttribute> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductSpecificationAttribute_Name");
        }
    }

    public class ProductOptionOverride : IAutoMappingOverride<ProductOption>
    {
        public void Override(AutoMapping<ProductOption> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductOption_Name");
            mapping.HasManyToMany(option => option.Products).Table("Ecommerce_ProductOptions").Inverse();
        }
    }

    public class ProductAttributeValueOverride : IAutoMappingOverride<ProductOptionValue>
    {
        public void Override(AutoMapping<ProductOptionValue> mapping)
        {
            mapping.Map(x => x.Value).Index("IX_ProductOptionValue_Value");
        }
    }
    


}