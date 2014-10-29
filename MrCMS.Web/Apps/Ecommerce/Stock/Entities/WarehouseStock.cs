using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Stock.Entities
{
    public class WarehouseStock : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual int StockLevel { get; set; }

        public virtual string ProductVariantName
        {
            get { return ProductVariant == null ? "None" : ProductVariant.FullName; }
        }

        public virtual string WarehouseName
        {
            get { return Warehouse == null ? "None" : Warehouse.Name; }
        }

        public virtual string ProductVariantSKU
        {
            get { return ProductVariant == null ? "" : ProductVariant.SKU; }
        }
    }
}