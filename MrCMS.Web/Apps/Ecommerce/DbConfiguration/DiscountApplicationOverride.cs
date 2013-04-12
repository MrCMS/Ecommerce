using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class DiscountApplicationOverride : IAutoMappingOverride<DiscountApplication>
    {
        public void Override(AutoMapping<DiscountApplication> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("DiscountApplicationType");
        }
    }
}