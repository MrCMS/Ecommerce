using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UKFirstClassShippingController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUKFirstClassShippingAdminService _ukFirstClassShippingAdminService;

        public UKFirstClassShippingController(IUKFirstClassShippingAdminService ukFirstClassShippingAdminService)
        {
            _ukFirstClassShippingAdminService = ukFirstClassShippingAdminService;
        }

        [HttpGet]
        public ViewResult Configure()
        {
            ViewData["tax-rate-options"] = _ukFirstClassShippingAdminService.GetTaxRateOptions();
            ViewData["calculations"] = _ukFirstClassShippingAdminService.GetCalculations();
            return View(_ukFirstClassShippingAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Configure(UKFirstClassShippingSettings settings)
        {
            _ukFirstClassShippingAdminService.Save(settings);
            TempData.SuccessMessages().Add("Settings updated");
            return RedirectToAction("Configure");
        }
    }
}