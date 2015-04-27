using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class BrandOverride : IAutoMappingOverride<Brand>
    {
        public void Override(AutoMapping<Brand> mapping)
        {
            mapping.Map(x => x.IsMigrated).Not.Nullable().Default("0");
        }
    }
}