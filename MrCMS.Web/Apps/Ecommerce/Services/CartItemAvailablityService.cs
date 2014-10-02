using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CartItemAvailablityService : ICartItemAvailablityService
    {
        private readonly IProductStockChecker _productStockChecker;

        public CartItemAvailablityService(IProductStockChecker productStockChecker)
        {
            _productStockChecker = productStockChecker;
        }
        public CanBuyStatus CanBuy(CartItem item)
        {
            var productVariant = item.Item;
            var quantity = item.Quantity;
            if (!_productStockChecker.IsInStock(productVariant))
                return new OutOfStock(productVariant);
            var canOrderQuantityResult = _productStockChecker.CanOrderQuantity(productVariant, quantity);
            if (!canOrderQuantityResult.CanOrder)
                return new CannotOrderQuantity(productVariant, canOrderQuantityResult.StockRemaining);
            return new CanBuy();
        }
    }
}