using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceDashboardController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IReportService _reportService;

        public EcommerceDashboardController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [DashboardAreaAction(DashboardArea = DashboardArea.Top, Order = 1)]
        [MrCMSACLRule(typeof(DashboardRevenueACL), DashboardRevenueACL.ShowRevenue)]
        public ActionResult Revenue()
        {
            return PartialView();
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult RevenueToday()
        {
            return Json(_reportService.SalesTodayGroupedByHour());
        }

        [HttpPost]
        public JsonResult RevenueThisWeek()
        {
            return Json(_reportService.SalesXDays());
        }
    }
}