using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ShippingMethodOverride : IAutoMappingOverride<ShippingMethod>
    {
        public void Override(AutoMapping<ShippingMethod> mapping)
        {
            mapping.HasMany(shippingMethod => shippingMethod.ShippingCalculations).Cascade.Delete();
        }
    }
}