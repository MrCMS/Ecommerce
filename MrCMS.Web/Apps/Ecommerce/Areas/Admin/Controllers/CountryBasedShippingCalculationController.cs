using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CountryBasedShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICountryBasedShippingCalculationAdminService _countryBasedShippingCalculationAdminService;

        public CountryBasedShippingCalculationController(
            ICountryBasedShippingCalculationAdminService countryBasedShippingCalculationAdminService)
        {
            _countryBasedShippingCalculationAdminService = countryBasedShippingCalculationAdminService;
        }

        [HttpGet]
        public ViewResult Add()
        {
            ViewData["criteria-options"] = _countryBasedShippingCalculationAdminService.GetCriteriaOptions();
            ViewData["country-options"] = _countryBasedShippingCalculationAdminService.GetCountryOptions();
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Add(
            [IoCModelBinder(typeof (CountryBasedShippingCalculationModelBinder))] CountryBasedShippingCalculation
                countryBasedShippingCalculation)
        {
            _countryBasedShippingCalculationAdminService.Add(countryBasedShippingCalculation);

            return RedirectToAction("Configure", "CountryBasedShipping");
        }

        [HttpGet]
        public ViewResult Edit(CountryBasedShippingCalculation calculation)
        {
            ViewData["criteria-options"] = _countryBasedShippingCalculationAdminService.GetCriteriaOptions();
            ViewData["country-options"] = _countryBasedShippingCalculationAdminService.GetCountryOptions();
            return View(calculation);
        }

        [HttpPost, ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(
            [IoCModelBinder(typeof (CountryBasedShippingCalculationModelBinder))] CountryBasedShippingCalculation
                countryBasedShippingCalculation)
        {
            _countryBasedShippingCalculationAdminService.Update(countryBasedShippingCalculation);

            return RedirectToAction("Configure", "CountryBasedShipping");
        }
    }
}