using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate.Mapping;

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
            mapping.Map(variant => variant.RestrictedTo).CustomType<BinaryData<HashSet<string>>>().Length(9999);       
        }
    }
}