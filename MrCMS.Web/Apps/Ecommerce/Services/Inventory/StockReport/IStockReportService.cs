namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.StockReport
{
    public interface IStockReportService
    {
        byte[] GenerateStockReport();
        byte[] GenerateLowStockReport(int threshold = 10);
    }
}