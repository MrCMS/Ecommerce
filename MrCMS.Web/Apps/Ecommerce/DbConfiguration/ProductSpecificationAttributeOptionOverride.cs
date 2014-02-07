using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductSpecificationAttributeOptionOverride : IAutoMappingOverride<ProductSpecificationAttributeOption>
    {
        public void Override(AutoMapping<ProductSpecificationAttributeOption> mapping)
        {
            mapping.Map(x => x.Name).Index("IX_ProductSpecificationAttributeOption_Name");
        }
    }
}