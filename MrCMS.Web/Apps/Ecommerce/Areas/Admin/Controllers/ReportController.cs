using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

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
        [MrCMSACLRule(typeof(SalesByDayACL), SalesByDayACL.View)]
        public ViewResult SalesByDay(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByDay")]
        [MrCMSACLRule(typeof(SalesByDayACL), SalesByDayACL.View)]
        public JsonResult SalesByDay_POST(ChartModel model)
        {
            return Json(_reportService.SalesByDay(model));
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SalesByPaymentTypeACL), SalesByPaymentTypeACL.View)]
        public ViewResult SalesByPaymentType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByPaymentType")]
        [MrCMSACLRule(typeof(SalesByPaymentTypeACL), SalesByPaymentTypeACL.View)]
        public JsonResult SalesByPaymentType_POST(ChartModel model)
        {
            return Json(_reportService.SalesByPaymentType(model));
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SalesByShippingTypeACL), SalesByShippingTypeACL.View)]
        public ViewResult SalesByShippingType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SalesByShippingType")]
        [MrCMSACLRule(typeof(SalesByShippingTypeACL), SalesByShippingTypeACL.View)]
        public JsonResult SalesByShippingType_POST(ChartModel model)
        {
            return Json(_reportService.SalesByShippingType(model));
        }

        [HttpGet]
        [MrCMSACLRule(typeof(OrdersByShippingTypeACL), OrdersByShippingTypeACL.View)]
        public ViewResult OrdersByShippingType(ChartModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("OrdersByShippingType")]
        [MrCMSACLRule(typeof(OrdersByShippingTypeACL), OrdersByShippingTypeACL.View)]
        public JsonResult OrdersByShippingType_POST(ChartModel model)
        {
            return Json(_reportService.OrdersByShippingType(model));
        }
    }
}