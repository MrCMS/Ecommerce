using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CartValidationService : ICartValidationService
    {
        private readonly IProductVariantAvailabilityService _productVariantAvailabilityService;
        private readonly CartModel _cart;

        public CartValidationService(IProductVariantAvailabilityService productVariantAvailabilityService, CartModel cart)
        {
            _productVariantAvailabilityService = productVariantAvailabilityService;
            _cart = cart;
        }

        public CanAddQuantityValidationResult CanAddQuantity(AddToCartModel model)
        {
            ProductVariant variant = model.ProductVariant;

            if (variant == null)
                return new CanAddQuantityValidationResult("Cannot find the selected variant");
            var quantity = GetRequestedQuantity(variant, model.Quantity);
            if (quantity <= 0)
                return new CanAddQuantityValidationResult("Cannot add an amount less than 1");
            CanBuyStatus canBuy = _productVariantAvailabilityService.CanBuy(variant, quantity);
            if (!canBuy.OK)
                return new CanAddQuantityValidationResult(canBuy.Message);
            return CanAddQuantityValidationResult.Successful;
        }

        public int GetRequestedQuantity(ProductVariant productVariant, int additionalQuantity)
        {
            var requestedQuantity = additionalQuantity;
            var existingItem = _cart.Items.FirstOrDefault(item => item.Item == productVariant);
            if (existingItem != null)
                requestedQuantity += existingItem.Quantity;
            return requestedQuantity;
        }
    }
}