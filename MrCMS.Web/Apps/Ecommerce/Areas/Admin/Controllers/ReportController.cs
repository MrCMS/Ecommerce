using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ReportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public ViewResult SalesByDay(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByDay")]
        public JsonResult SalesByDay_POST(ChartModel model)
        {
            return Json(_reportService.SalesByDay(model));
        }

        [HttpPost]
        public JsonResult SalesByDayStructure(ChartModel model)
        {
            return Json(_reportService.SalesByDayStructure(model));
        }
    }
}