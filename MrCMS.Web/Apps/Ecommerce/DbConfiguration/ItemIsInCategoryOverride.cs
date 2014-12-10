using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ItemIsInCategoryOverride : IAutoMappingOverride<ItemIsInCategory>
    {
        public void Override(AutoMapping<ItemIsInCategory> mapping)
        {
            mapping.Map(x => x.CategoryIds).MakeVarCharMax();
        }
    }
}