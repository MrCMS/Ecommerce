using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CountryOverride : IAutoMappingOverride<Country>
    {
        public void Override(AutoMapping<Country> mapping)
        {
            mapping.HasMany(country => country.ShippingCalculations).Cascade.Delete();
        }
    }
}