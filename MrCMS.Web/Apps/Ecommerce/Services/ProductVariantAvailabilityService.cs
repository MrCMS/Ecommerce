using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductVariantAvailabilityService : IProductVariantAvailabilityService
    {
        private readonly IProductStockChecker _productStockChecker;
        private readonly IProductShippingChecker _productShippingChecker;
        //private readonly CartModel _cart;

        public ProductVariantAvailabilityService(IProductStockChecker productStockChecker,
            IProductShippingChecker productShippingChecker)
        {
            _productStockChecker = productStockChecker;
            _productShippingChecker = productShippingChecker;
        }

        public CanBuyStatus CanBuy(ProductVariant productVariant, int quantity)
        {
            if (!_productStockChecker.IsInStock(productVariant))
                return new OutOfStock(productVariant);
            if (!_productStockChecker.CanOrderQuantity(productVariant, quantity))
                return new CannotOrderQuantity(productVariant, quantity);
            if (!_productShippingChecker.CanShip(productVariant))
                return new NoShippingMethodWouldBeAvailable(productVariant);
            return new CanBuy();
        }
    }
}