using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IChartService _chartService;
        private readonly IOrderAnalyticsService _orderAnalyticsService;

        public ReportService(IChartService chartService, IOrderAnalyticsService orderAnalyticsService)
        {
            _chartService = chartService;
            _orderAnalyticsService = orderAnalyticsService;
        }

        public ChartModel SalesByDay(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueGroupedByDate(model.From,model.To);
            _chartService.SetLineChartData(ref model,data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesByPaymentType(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueGrouped("PaymentMethod",model.From, model.To);
            _chartService.SetBarChartLabelsAndData(ref model, data);
            return model;
        }

        public ChartModel SalesByShippingType(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueGrouped("ShippingMethod", model.From, model.To);
            _chartService.SetBarChartLabelsAndData(ref model, data);
            return model;
        }
    }
}