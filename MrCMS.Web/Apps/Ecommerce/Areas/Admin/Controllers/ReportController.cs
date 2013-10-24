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

        [HttpGet]
        public ViewResult SalesByPaymentType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByPaymentType")]
        public JsonResult SalesByPaymentType_POST(ChartModel model)
        {
            return Json(_reportService.SalesByPaymentType(model));
        }

        [HttpGet]
        public ViewResult SalesByShippingType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByShippingType")]
        public JsonResult SalesByShippingType_POST(ChartModel model)
        {
            return Json(_reportService.SalesByShippingType(model));
        }

        [HttpGet]
        public ViewResult OrdersByShippingType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("OrdersByShippingType")]
        public JsonResult OrdersByShippingType_POST(ChartModel model)
        {
            return Json(_reportService.OrdersByShippingType(model));
        }
    }
}