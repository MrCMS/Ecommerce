using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class GenerateWarehouseStock :IOnAdded<ProductVariant>
    {
        private readonly EcommerceSettings _ecommerceSettings;

        public GenerateWarehouseStock(EcommerceSettings ecommerceSettings)
        {
            _ecommerceSettings = ecommerceSettings;
        }

        public void Execute(OnAddedArgs<ProductVariant> args)
        {
            if (!_ecommerceSettings.WarehouseStockEnabled)
                return;
            var productVariant = args.Item;

            var session = args.Session; 
            var warehouses = session.QueryOver<Warehouse>().Cacheable().List();
            session.Transact(ses => warehouses.ForEach(warehouse => ses.Save(new WarehouseStock
            {
                StockLevel = 0,
                ProductVariant = productVariant,
                Warehouse = warehouse
            })));
        }
    }
}