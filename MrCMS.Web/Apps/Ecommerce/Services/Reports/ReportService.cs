using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics;

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
            var data = _orderAnalyticsService.GetRevenueGroupedBySalesChannel(model.From,model.To);
            _chartService.SetLineChartData(ref model,data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesByDayStructure(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueStructureGroupedBySalesChannel(model.From, model.To);
            _chartService.SetPieChartLabelsAndData(ref model, data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesByPaymentType(ChartModel model)
        {
            return null;
        }

        public ChartModel SalesByShippingType(ChartModel model)
        {
            return null;
        }
    }
}