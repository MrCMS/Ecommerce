using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class DiscountLimitationOverride : IAutoMappingOverride<DiscountLimitation>
    {
        public void Override(AutoMapping<DiscountLimitation> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("DiscountLimitationType");
        }
    }
}