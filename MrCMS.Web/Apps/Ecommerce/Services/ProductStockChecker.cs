using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductStockChecker : IProductStockChecker
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;

        public ProductStockChecker(ISession session, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public bool IsInStock(ProductVariant productVariant)
        {
            if (productVariant.TrackingPolicy == TrackingPolicy.DontTrack)
                return true;
            return _ecommerceSettings.WarehouseStockEnabled
                ? _session.QueryOver<WarehouseStock>()
                    .Where(stock => stock.ProductVariant.Id == productVariant.Id && stock.StockLevel > 0)
                    .Any()
                : productVariant.StockRemaining > 0;
        }

        public bool CanOrderQuantity(ProductVariant productVariant, int quantity)
        {
            if (productVariant.TrackingPolicy == TrackingPolicy.DontTrack)
                return true;

            return _ecommerceSettings.WarehouseStockEnabled
                ? _session.QueryOver<WarehouseStock>()
                    .Where(stock => stock.ProductVariant.Id == productVariant.Id && stock.StockLevel > 0)
                    .Select(Projections.Sum<WarehouseStock>(warehouseStock => warehouseStock.StockLevel))
                    .SingleOrDefault<int>() >= quantity
                : productVariant.StockRemaining >= quantity;
        }
    }
}