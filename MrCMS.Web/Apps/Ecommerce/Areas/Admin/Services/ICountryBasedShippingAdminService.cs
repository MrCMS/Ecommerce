using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICountryBasedShippingAdminService
    {
        List<SelectListItem> GetTaxRateOptions();
        List<CountryBasedShippingCalculation> GetCalculations();
        CountryBasedShippingSettings GetSettings();
        void Save(CountryBasedShippingSettings settings);
    }
}