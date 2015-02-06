using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ItemHasSKUOverride : IAutoMappingOverride<ItemHasSKU>
    {
        public void Override(AutoMapping<ItemHasSKU> mapping)
        {
            mapping.Map(x => x.SKUs).MakeVarCharMax();
        }
    }
}