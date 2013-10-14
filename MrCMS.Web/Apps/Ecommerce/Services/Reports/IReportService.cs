using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public interface IReportService
    {
        ChartModel SalesByDay(ChartModel model);
        ChartModel SalesByDayStructure(ChartModel model);
        ChartModel SalesByPaymentType(ChartModel model);
        ChartModel SalesByShippingType(ChartModel model);
    }
}