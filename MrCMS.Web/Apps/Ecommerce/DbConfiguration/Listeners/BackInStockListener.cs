using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners
{
    public class BackInStockListener : IOnUpdated<ProductVariant>, IOnUpdated<WarehouseStock>
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ISession _session;

        public BackInStockListener(ISession session, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public void Execute(OnUpdatedArgs<ProductVariant> args)
        {
            // if warehouse stock is enabled, it's not relevant
            if (_ecommerceSettings.WarehouseStockEnabled)
                return;

            var productVariant = args.Original;
            if (productVariant == null)
                return;


            var oldValue = args.Original.StockRemaining;
            if (oldValue > 0)
                return;

            var newValue = args.Item.StockRemaining;
            if (newValue <= 0)
                return;

            if (newValue > oldValue)
                RaiseBackInStockNotification(productVariant);
        }

        public void Execute(OnUpdatedArgs<WarehouseStock> args)
        {
            // if warehouse stock is not enabled, it's not relevant
            if (!_ecommerceSettings.WarehouseStockEnabled)
                return;
            // these first two checks are saying that either if there was stock, before, 
            // or there still isn't stock, it's definitely not back in stock.
            var oldValue = args.Original.StockLevel;
            if (oldValue > 0)
                return;

            var newValue = args.Item.StockLevel;
            if (newValue <= 0)
                return;

            var stockItem = args.Item;
            var variant = stockItem.ProductVariant;

            // We check the other warehouses, to see if it was out of stock everywhere, or just here
            var otherWarehousesWithStock = _session.QueryOver<WarehouseStock>()
                .Where(
                    stock => stock.ProductVariant.Id == variant.Id && stock.StockLevel > 0 && stock.Id != stockItem.Id)
                .Cacheable()
                .Any();

            if (!otherWarehousesWithStock)
                RaiseBackInStockNotification(variant);
        }

        private void RaiseBackInStockNotification(ProductVariant productVariant)
        {
            _session.Transact(sess =>
            {
                var variant = sess.Get<ProductVariant>(productVariant.Id);
                var backInStockProductVariant = new BackInStockProductVariant
                {
                    ProductVariant = variant,
                    CreatedOn = CurrentRequestData.Now,
                    UpdatedOn = CurrentRequestData.Now
                };
                sess.Save(backInStockProductVariant);
            });
        }
    }
}