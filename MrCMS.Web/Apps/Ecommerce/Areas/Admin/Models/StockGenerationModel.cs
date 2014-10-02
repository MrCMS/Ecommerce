using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class StockGenerationModel
    {
        public int WarehouseId { get; set; }
        [DisplayName("How do you want to generate stock?")]
        public StockGenerationType StockGenerationType { get; set; }
        [DisplayName("Warehouse")]
        public int WarehouseToCopyId { get; set; }
        [DisplayName("Fixed Value")]
        public int FixedValue { get; set; }
    }
}