using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CartItemBasedDiscountApplicationOverride : IAutoMappingOverride<CartItemBasedDiscountApplication>
    {
        public void Override(AutoMapping<CartItemBasedDiscountApplication> mapping)
        {
            mapping.Map(x => x.SKUs).MakeVarCharMax();
            mapping.Map(x => x.CategoryIds).MakeVarCharMax();
        }
    }
}