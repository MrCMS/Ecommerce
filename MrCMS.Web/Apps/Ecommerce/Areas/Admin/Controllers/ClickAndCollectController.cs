using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ClickAndCollectController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IClickAndCollectAdminService _adminService;

        public ClickAndCollectController(IClickAndCollectAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public ViewResult Configure()
        {
            return View(_adminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Configure(ClickAndCollectSettings settings)
        {
            _adminService.Save(settings);
            TempData.SuccessMessages().Add("Settings updated");
            return RedirectToAction("Configure");
        }
    }
}