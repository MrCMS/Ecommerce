using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Stock.Entities
{
    public class WarehouseStock : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual int StockLevel { get; set; }
    }
}