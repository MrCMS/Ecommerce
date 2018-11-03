using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ItemBrandIsNotOverride : IAutoMappingOverride<ItemBrandIsNot>
    {
        public void Override(AutoMapping<ItemBrandIsNot> mapping)
        {
            mapping.Map(x => x.Brands).MakeVarCharMax();
        }
    }
}