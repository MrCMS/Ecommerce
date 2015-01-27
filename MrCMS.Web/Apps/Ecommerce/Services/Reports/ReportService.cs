using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Analytics.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IChartService _chartService;
        private readonly IOrderAnalyticsService _orderAnalyticsService;
        private readonly EcommerceSettings _ecommerceSettings;

        public ReportService(IChartService chartService, IOrderAnalyticsService orderAnalyticsService, EcommerceSettings ecommerceSettings)
        {
            _chartService = chartService;
            _orderAnalyticsService = orderAnalyticsService;
            _ecommerceSettings = ecommerceSettings;
        }

        public ChartModel SalesByDay(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueGroupedByDate(model.From, model.To);
            _chartService.SetLineChartData(ref model, data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesByPaymentType(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueByPaymentMethod(model.From, model.To);
            _chartService.SetBarChartLabelsAndData(ref model, data);
            return model;
        }

        public ChartModel SalesByShippingType(ChartModel model)
        {
            var data = _orderAnalyticsService.GetRevenueByShippingMethod(model.From, model.To);
            _chartService.SetBarChartLabelsAndData(ref model, data);
            return model;
        }

        public ChartModel SalesTodayGroupedByHour()
        {
            var data = _orderAnalyticsService.GetRevenueForTodayGroupedByHour();
            var model = new ChartModel { From = CurrentRequestData.Now.Date, To = CurrentRequestData.Now.Date.AddDays(1) };
            _chartService.SetLineChartData(ref model, data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesLastWeekGroupedByDay()
        {
            var model = new ChartModel() { From = CurrentRequestData.Now.Date.AddDays(-6), To = CurrentRequestData.Now.Date.AddDays(1) };
            var data = _orderAnalyticsService.GetRevenueGroupedByDate(model.From, model.To);
            _chartService.SetLineChartData(ref model, data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel SalesXDays()
        {
            var days = _ecommerceSettings.DashboardRevenueDays;
            if (days <= 0)
                days = 7;

            var model = new ChartModel() { From = CurrentRequestData.Now.Date.AddDays(-(days - 1)), To = CurrentRequestData.Now.Date.AddDays(1) };
            var data = _orderAnalyticsService.GetRevenueGroupedByDate(model.From, model.To);
            _chartService.SetLineChartData(ref model, data);
            _chartService.SetLineChartLabels(ref model);
            return model;
        }

        public ChartModel OrdersByShippingType(ChartModel model)
        {
            var data = _orderAnalyticsService.GetOrdersGrouped(model.From, model.To);
            _chartService.SetBarChartLabelsAndData(ref model, data);
            return model;
        }
    }
}