using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UKCourierShippingAdminService : IUKCourierShippingAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IGetTaxRateOptions _taxRateOptions;
        private readonly ISession _session;

        public UKCourierShippingAdminService(IConfigurationProvider configurationProvider, IGetTaxRateOptions taxRateOptions,ISession session)
        {
            _configurationProvider = configurationProvider;
            _taxRateOptions = taxRateOptions;
            _session = session;
        }

        public UKCourierShippingSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<UKCourierShippingSettings>();
        }

        public List<SelectListItem> GetTaxRateOptions()
        {
            return _taxRateOptions.GetOptions(GetSettings().TaxRateId);
        }

        public void Save(UKCourierShippingSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }

        public List<UKCourierShippingCalculation> GetCalculations()
        {
            return _session.QueryOver<UKCourierShippingCalculation>().Cacheable().List().ToList();
        }
    }
}