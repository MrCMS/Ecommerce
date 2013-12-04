using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductAttributeValueOverride : IAutoMappingOverride<ProductOptionValue>
    {
        public void Override(AutoMapping<ProductOptionValue> mapping)
        {
            mapping.Map(x => x.Value).Index("IX_ProductOptionValue_Value");
        }
    }
}