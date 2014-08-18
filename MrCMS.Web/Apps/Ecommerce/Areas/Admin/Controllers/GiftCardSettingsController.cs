using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class GiftCardSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IGiftCardSettingsAdminService _giftCardSettingsAdminService;

        public GiftCardSettingsController(IGiftCardSettingsAdminService giftCardSettingsAdminService)
        {
            _giftCardSettingsAdminService = giftCardSettingsAdminService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            ViewData["activation-status-options"] = _giftCardSettingsAdminService.GetActivationStatusOptions();
            return View(_giftCardSettingsAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Index(GiftCardSettings settings)
        {
            _giftCardSettingsAdminService.SaveSettings(settings);
            TempData.SuccessMessages().Add("Settings saved");
            return RedirectToAction("Index", "GiftCard");
        }
    }
}