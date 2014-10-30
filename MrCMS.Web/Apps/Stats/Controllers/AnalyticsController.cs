using System;
using System.Web.Mvc;
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

        public JsonResult LogPageView(PageViewInfo info)
        {
            _logPageViewService.LogPageView(info);
            return Json(true);
        }
    }

    public class PageViewInfo
    {
        public Guid User { get; set; }
        public Guid Session { get; set; }
        public string Url { get; set; }
    }
}