using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DashBoardRevenueSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly DashBoardRevenueSettings _dashBoardRevenueSettings;

        public DashBoardRevenueSettingsController(IConfigurationProvider configurationProvider, DashBoardRevenueSettings dashBoardRevenueSettings)
        {
            _configurationProvider = configurationProvider;
            _dashBoardRevenueSettings = dashBoardRevenueSettings;
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return View(_dashBoardRevenueSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(DashBoardRevenueSettings dashBoardRevenueSettings)
        {
            _configurationProvider.SaveSettings(dashBoardRevenueSettings);
            return RedirectToAction("Edit");
        }
    }
}