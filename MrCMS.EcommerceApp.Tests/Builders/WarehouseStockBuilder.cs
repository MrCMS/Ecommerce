using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class WarehouseStockBuilder : IEntityBuilder<WarehouseStock>
    {
        private readonly Warehouse _warehouse;
        private readonly ProductVariant _productVariant;
        private int _stockLevel = 0;

        public WarehouseStockBuilder(ProductVariant productVariant, Warehouse warehouse)
        {
            _productVariant = productVariant;
            _warehouse = warehouse;
        }

        public WarehouseStockBuilder WithStockLevel(int stockLevel)
        {
            _stockLevel = stockLevel;
            return this;
        }

        public WarehouseStock Build()
        {
            return new WarehouseStock
            {
                Warehouse = _warehouse,
                ProductVariant = _productVariant,
                StockLevel = _stockLevel
            };
        }
    }
}