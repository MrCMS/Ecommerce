using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CountryBasedShippingCalculationOverride : IAutoMappingOverride<CountryBasedShippingCalculation>
    {
        public void Override(AutoMapping<CountryBasedShippingCalculation> mapping)
        {
            mapping.Map(calculation => calculation.Countries).MakeVarCharMax();
        }
    }
}