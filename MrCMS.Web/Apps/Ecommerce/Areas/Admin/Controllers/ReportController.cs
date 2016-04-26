using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ReportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IReportService _reportService;
        private readonly IOrderAnalyticsService _orderAnalyticsService;

        public ReportController(IReportService reportService, IOrderAnalyticsService orderAnalyticsService)
        {
            _reportService = reportService;
            _orderAnalyticsService = orderAnalyticsService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SalesByDayACL), SalesByDayACL.View)]
        public ViewResult SalesByDay(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueWithOrdersGroupedByDate(model.From, model.To);

            var viewModel = new SalesByDayViewModel
            {
                From = model.From,
                To = model.To,
                Sales = data.ToList()
            };

            return View(viewModel);
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