using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class ApiController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonLogService _amazonLogService;

        public ApiController(IAmazonLogService amazonLogService)
        {
            _amazonLogService = amazonLogService;
        }

        [HttpGet]
        public ViewResult Dashboard()
        {
            return View();
        }

        public ActionResult DashboardLogs(int page = 1)
        {
            var model = new AmazonDashboardModel()
            {
                Logs = _amazonLogService.GetEntriesPaged(page)
            };
            return PartialView(model);
        }
    }
}
