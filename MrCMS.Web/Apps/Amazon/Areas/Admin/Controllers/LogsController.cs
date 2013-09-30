using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class LogsController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonLogService _amazonLogService;

        public LogsController(IAmazonLogService amazonLogService)
        {
            _amazonLogService = amazonLogService;
        }

        [HttpGet]
        public ViewResult Index(AmazonLogType? type, AmazonLogStatus? status, int page = 1)
        {
            var model = _amazonLogService.GetEntriesPaged(page, type, status);
            return View(model);
        }

        [HttpPost]
        [ActionName("Index")]
        public RedirectToRouteResult Index_POST(AmazonLogType? type, AmazonLogStatus? status)
        {
            return RedirectToAction("Index", new { type,status });
        }

        public ViewResult Details(AmazonLog entry)
        {
            return View(entry);
        }

        [HttpGet]
        public ActionResult DeleteAllLogs()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("DeleteAllLogs")]
        public ActionResult DeleteAllLogs_POST()
        {
            _amazonLogService.DeleteAllLogs();
            _amazonLogService.Add(AmazonLogType.Logs, AmazonLogStatus.Delete, null, null, null, null, null, null, null, null);
            return RedirectToAction("Index");
        }
    }
}
