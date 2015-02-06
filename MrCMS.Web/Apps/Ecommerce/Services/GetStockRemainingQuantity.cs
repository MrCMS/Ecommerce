using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetStockRemainingQuantity : IGetStockRemainingQuantity
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;

        public GetStockRemainingQuantity(ISession session, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public int Get(ProductVariant productVariant)
        {
            return _ecommerceSettings.WarehouseStockEnabled
                ? _session.QueryOver<WarehouseStock>()
                    .Where(stock => stock.ProductVariant.Id == productVariant.Id && stock.StockLevel > 0)
                    .Select(Projections.Sum<WarehouseStock>(warehouseStock => warehouseStock.StockLevel))
                    .Cacheable()
                    .SingleOrDefault<int>()
                : productVariant.StockRemaining;
        }
    }
}