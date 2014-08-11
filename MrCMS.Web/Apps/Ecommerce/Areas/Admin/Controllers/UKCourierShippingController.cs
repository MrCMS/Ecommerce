using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
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
            ViewData["calculations"] = _ukCourierShippingAdminService.GetCalculations();
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
}