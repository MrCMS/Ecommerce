namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class BulkWarehouseStockUpdateDTO
    {
        public string SKU { get; set; }
        public int WarehouseId { get; set; }
        public int StockLevel { get; set; }
    }
}