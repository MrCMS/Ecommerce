using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CountryOverride : IAutoMappingOverride<Country>
    {
        public void Override(AutoMapping<Country> mapping)
        {
        }
    }
}