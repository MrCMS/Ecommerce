using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class BrandOverride : IAutoMappingOverride<OldBrand>
    {
        public void Override(AutoMapping<OldBrand> mapping)
        {
            mapping.Table("Ecommerce_Brand");
            mapping.Map(x => x.IsMigrated).Not.Nullable().Default("0");
        }
    }
}