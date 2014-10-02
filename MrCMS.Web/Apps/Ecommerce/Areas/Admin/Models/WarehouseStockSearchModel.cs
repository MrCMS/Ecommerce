using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class WarehouseStockSearchModel
    {
        public WarehouseStockSearchModel()
        {
            Page = 1;
        }
        public int Page { get; set; }

        [DisplayName("Warehouse")]
        public int? WarehouseId { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }
    }
}