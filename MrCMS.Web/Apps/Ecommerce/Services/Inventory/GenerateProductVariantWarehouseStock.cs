using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public class GenerateProductVariantWarehouseStock : IGenerateProductVariantWarehouseStock
    {
        private readonly ISession _session;

        public GenerateProductVariantWarehouseStock(ISession session)
        {
            _session = session;
        }

        public void Generate(ProductVariant productVariant)
        {
            var warehouses = _session.QueryOver<Warehouse>().Cacheable().List();
            _session.Transact(ses => warehouses.ForEach(warehouse => ses.Save(new WarehouseStock
            {
                StockLevel = 0,
                ProductVariant = productVariant,
                Warehouse = warehouse
            })));
        }
    }
}