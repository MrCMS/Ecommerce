using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CountryBasedShippingController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICountryBasedShippingAdminService _countryBasedShippingAdminService;

        public CountryBasedShippingController(ICountryBasedShippingAdminService countryBasedShippingAdminService)
        {
            _countryBasedShippingAdminService = countryBasedShippingAdminService;
        }

        [HttpGet]
        public ViewResult Configure()
        {
            ViewData["tax-rate-options"] = _countryBasedShippingAdminService.GetTaxRateOptions();
            ViewData["calculations"] = _countryBasedShippingAdminService.GetCalculations();
            return View(_countryBasedShippingAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Configure(CountryBasedShippingSettings settings)
        {
            _countryBasedShippingAdminService.Save(settings);
            TempData.SuccessMessages().Add("Settings updated");
            return RedirectToAction("Configure");
        }
    }
}