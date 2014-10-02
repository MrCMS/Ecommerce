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
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;

        public ProductStockChecker(IGetStockRemainingQuantity getStockRemainingQuantity)
        {
            _getStockRemainingQuantity = getStockRemainingQuantity;
        }

        public bool IsInStock(ProductVariant productVariant)
        {
            if (productVariant.TrackingPolicy == TrackingPolicy.DontTrack)
                return true;
            return _getStockRemainingQuantity.Get(productVariant) > 0;
        }

        public CanOrderQuantityResult CanOrderQuantity(ProductVariant productVariant, int quantity)
        {
            if (productVariant.TrackingPolicy == TrackingPolicy.DontTrack)
                return new CanOrderQuantityResult {CanOrder = true};

            var stockRemaining = _getStockRemainingQuantity.Get(productVariant);
            return new CanOrderQuantityResult
            {
                CanOrder = stockRemaining >= quantity,
                StockRemaining = stockRemaining
            };
        }
    }
}