using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class CountryBasedShippingAdminService : ICountryBasedShippingAdminService
    {
        private readonly IGetTaxRateOptions _getTaxRateOptions;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISession _session;

        public CountryBasedShippingAdminService(IGetTaxRateOptions getTaxRateOptions, IConfigurationProvider configurationProvider,ISession session)
        {
            _getTaxRateOptions = getTaxRateOptions;
            _configurationProvider = configurationProvider;
            _session = session;
        }

        public List<SelectListItem> GetTaxRateOptions()
        {
            var settings = GetSettings();
            return _getTaxRateOptions.GetOptions(settings.TaxRateId);
        }

        public List<CountryBasedShippingCalculation> GetCalculations()
        {
            return _session.QueryOver<CountryBasedShippingCalculation>().Cacheable().List().ToList();
        }

        public CountryBasedShippingSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<CountryBasedShippingSettings>();
        }

        public void Save(CountryBasedShippingSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }
    }
}