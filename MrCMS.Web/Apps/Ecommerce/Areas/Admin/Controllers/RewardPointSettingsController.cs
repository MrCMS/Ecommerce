using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class RewardPointSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IRewardPointSettingsAdminService _rewardPointSettingsAdminService;

        public RewardPointSettingsController(IRewardPointSettingsAdminService rewardPointSettingsAdminService)
        {
            _rewardPointSettingsAdminService = rewardPointSettingsAdminService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View(_rewardPointSettingsAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Index(RewardPointSettings settings)
        {
            _rewardPointSettingsAdminService.SaveSettings(settings);
            return RedirectToAction("Index", "RewardPointSettings");
        }
    }
}