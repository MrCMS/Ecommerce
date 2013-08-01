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
            mapping.HasMany(variant => variant.AttributeValues).KeyColumn("ProductVariantId").Cascade.All();
            mapping.HasOne(variant => variant.GoogleBaseProduct)
                   .PropertyRef(product => product.ProductVariant)
                   .Cascade.All();
            mapping.HasMany(variant => variant.PriceBreaks).Cascade.All();

            mapping.Map(x => x.SKU).Index("IX_ProductVariant_SKU");
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

    public class ProductAttributeOptionOverride : IAutoMappingOverride<ProductAttributeOption>
    {
        public void Override(AutoMapping<ProductAttributeOption> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductAttributeOption_Name");
        }
    }

    public class ProductAttributeValueOverride : IAutoMappingOverride<ProductAttributeValue>
    {
        public void Override(AutoMapping<ProductAttributeValue> mapping)
        {
            mapping.Map(x => x.Value).Index("IX_ProductAttributeValue_Value");
        }
    }
    


}