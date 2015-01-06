using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICountryBasedShippingCalculationAdminService
    {
        List<SelectListItem> GetCriteriaOptions();
        List<SelectListItem> GetCountryOptions();

        void Add(CountryBasedShippingCalculation countryBasedShippingCalculation);
        void Update(CountryBasedShippingCalculation countryBasedShippingCalculation);
    }
}