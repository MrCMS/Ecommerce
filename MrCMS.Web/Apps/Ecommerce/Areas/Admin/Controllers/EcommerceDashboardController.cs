using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using NHibernate;

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