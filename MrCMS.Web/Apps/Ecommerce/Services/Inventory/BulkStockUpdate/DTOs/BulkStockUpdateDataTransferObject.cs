namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs
{
    public class BulkStockUpdateDataTransferObject 
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public int? StockRemaining { get; set; }
    }
}