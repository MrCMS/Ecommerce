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
    public class UKFirstClassShippingAdminService : IUKFirstClassShippingAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IGetTaxRateOptions _getTaxRateOptions;
        private readonly ISession _session;

        public UKFirstClassShippingAdminService(IConfigurationProvider configurationProvider,IGetTaxRateOptions getTaxRateOptions,ISession session)
        {
            _configurationProvider = configurationProvider;
            _getTaxRateOptions = getTaxRateOptions;
            _session = session;
        }

        public UKFirstClassShippingSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<UKFirstClassShippingSettings>();
        }

        public List<SelectListItem> GetTaxRateOptions()
        {
            return _getTaxRateOptions.GetOptions(GetSettings().TaxRateId);
        }

        public void Save(UKFirstClassShippingSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }

        public List<UKFirstClassShippingCalculation> GetCalculations()
        {
            return _session.QueryOver<UKFirstClassShippingCalculation>().Cacheable().List().ToList();
        }
    }
}