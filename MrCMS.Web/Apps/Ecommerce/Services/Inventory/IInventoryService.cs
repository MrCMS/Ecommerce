namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IInventoryService
    {
        byte[] ExportLowStockReport(int treshold = 10);
    }
}