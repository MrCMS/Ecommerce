using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceDashboardController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IUserService _userServices;
        private readonly ISession _session;
        private readonly IReportService _reportService;

        public EcommerceDashboardController(ICurrentSiteLocator currentSiteLocator, IUserService userServices, ISession session, IReportService reportService)
        {
            _currentSiteLocator = currentSiteLocator;
            _userServices = userServices;
            _session = session;
            _reportService = reportService;
        }

        
        [HttpGet]
        [DashboardAreaAction(DashboardArea = DashboardArea.Top, Order = 1)]
        public ActionResult Revenue()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult RevenueToday()
        {
            return Json(_reportService.SalesTodayGroupedByHour());
        }

        [HttpPost]
        public JsonResult RevenueThisWeek()
        {
            return Json(_reportService.SalesLastWeekGroupedByDay());
        }
    }
}