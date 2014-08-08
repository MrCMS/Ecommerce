using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UKCourierShippingController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUKCourierShippingAdminService _ukCourierShippingAdminService;

        public UKCourierShippingController(IUKCourierShippingAdminService ukCourierShippingAdminService)
        {
            _ukCourierShippingAdminService = ukCourierShippingAdminService;
        }

        [HttpGet]
        public ViewResult Configure()
        {
            ViewData["tax-rate-options"] = _ukCourierShippingAdminService.GetTaxRateOptions();
            return View(_ukCourierShippingAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Configure(UKCourierShippingSettings settings)
        {
            _ukCourierShippingAdminService.Save(settings);
            TempData.SuccessMessages().Add("Settings updated");
            return RedirectToAction("Configure");
        }
    }

    public interface IUKCourierShippingAdminService
    {
        UKCourierShippingSettings GetSettings();
        List<SelectListItem> GetTaxRateOptions();
        void Save(UKCourierShippingSettings settings);
    }

    public class UKCourierShippingAdminService : IUKCourierShippingAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IGetTaxRateOptions _taxRateOptions;

        public UKCourierShippingAdminService(IConfigurationProvider configurationProvider, IGetTaxRateOptions taxRateOptions)
        {
            _configurationProvider = configurationProvider;
            _taxRateOptions = taxRateOptions;
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
    }
}