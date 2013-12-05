using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CartValidationService : ICartValidationService
    {
        private readonly CartModel _cart;

        public CartValidationService(CartModel cart)
        {
            _cart = cart;
        }

        public CanAddQuantityValidationResult CanAddQuantity(AddToCartModel model)
        {
            if (model.ProductVariant == null)
                return new CanAddQuantityValidationResult("Cannot find the selected variant");
            if (model.Quantity <= 0)
                return new CanAddQuantityValidationResult("Cannot add an amount less than 1");
            var canBuy = model.ProductVariant.CanBuy(_cart, model.Quantity);
            if (!canBuy.OK)
                return new CanAddQuantityValidationResult(canBuy.Message);
            return CanAddQuantityValidationResult.Successful;
        }
    }
}