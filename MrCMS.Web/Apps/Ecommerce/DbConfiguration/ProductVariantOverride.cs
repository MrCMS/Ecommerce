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
            mapping.Map(x => x.BasePrice).Scale(2);
            mapping.Map(x => x.PreviousPrice).Scale(2);
            mapping.HasMany(variant => variant.OptionValues).KeyColumn("ProductVariantId").Cascade.All();
            mapping.HasMany(variant => variant.PriceBreaks).Cascade.All();

            mapping.Map(variant => variant.SKU).Index("IX_ProductVariant_SKU");

            mapping.HasManyToMany(variant => variant.RestrictedShippingMethods)
                .Table("Ecommerce_ProductVariantRestrictedShippingMethods")
                .Not.Inverse().Cache.ReadWrite();
        }
    }
}