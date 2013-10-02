using System.Web.Mvc;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ryness.Areas.Admin.Controllers
{
    public class KerridgeController : MrCMSAppAdminController<RynessApp>
    {
        private readonly IKerridgeService _kerridgeService;

        public KerridgeController(IKerridgeService kerridgeService)
        {
            _kerridgeService = kerridgeService;
        }

        public ViewResult Index(string q = null, int p = 1)
        {
            var kerridgeLog = _kerridgeService.Search(q, p);
            return View(kerridgeLog);
        }

    }
}