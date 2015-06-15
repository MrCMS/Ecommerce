using System.Web.Mvc;
using MrCMS.Web.Apps.Stats.Filters;
using MrCMS.Web.Apps.Stats.Models;
using MrCMS.Web.Apps.Stats.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Stats.Controllers
{
    public class AnalyticsController : MrCMSAppUIController<StatsApp>
    {
        private readonly ILogPageViewService _logPageViewService;

        public AnalyticsController(ILogPageViewService logPageViewService)
        {
            _logPageViewService = logPageViewService;
        }

        [PreventBots]
        public JsonResult LogPageView(PageViewInfo info)
        {
            _logPageViewService.LogPageView(info);
            return Json(true);
        }
    }
}