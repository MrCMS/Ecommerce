using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductVariantAvailabilityService : IProductVariantAvailabilityService
    {
        private readonly CartModel _cart;

        public ProductVariantAvailabilityService(CartModel cart)
        {
            _cart = cart;
        }

        public CanBuyStatus CanBuy(ProductVariant productVariant, int additionalQuantity = 0)
        {
            if (!productVariant.InStock)
                return new OutOfStock(productVariant);
            var requestedQuantity = GetRequestedQuantity(productVariant, additionalQuantity);
            if (productVariant.TrackingPolicy == TrackingPolicy.Track && requestedQuantity > productVariant.StockRemaining)
                return new CannotOrderQuantity(productVariant, requestedQuantity);
            if (productVariant.HasRestrictedShipping && !_cart.PotentiallyAvailableShippingMethods.Any(method => productVariant.RestrictedTo.Contains(method.TypeName)))
                return new NoShippingMethodWouldBeAvailable(productVariant);
            return new CanBuy();
        }

        private int GetRequestedQuantity(ProductVariant productVariant, int additionalQuantity)
        {
            var requestedQuantity = additionalQuantity;
            var existingItem = _cart.Items.FirstOrDefault(item => item.Item == productVariant);
            if (existingItem != null)
                requestedQuantity += existingItem.Quantity;
            return requestedQuantity;
        }
    }
}