using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class AddToCartUIService : IAddToCartUIService
    {
        private readonly ICartValidationService _cartValidationService;
        private readonly ICartItemManager _cartItemManager;
        private readonly IUniquePageService _uniquePageService;

        public AddToCartUIService(ICartValidationService cartValidationService, ICartItemManager cartItemManager,
            IUniquePageService uniquePageService)
        {
            _cartValidationService = cartValidationService;
            _cartItemManager = cartItemManager;
            _uniquePageService = uniquePageService;
        }

        public RedirectResult Redirect(AddToCartResult addToCartResult)
        {
            if (!addToCartResult.Success) 
                return _uniquePageService.RedirectTo<ProductSearch>();

            // redirect to product added to cart page if it exists
            var addedToCart = _uniquePageService.GetUniquePage<ProductAddedToCart>();
            if (addedToCart != null && addedToCart.Published)
                return _uniquePageService.RedirectTo<ProductAddedToCart>(
                    new { id = addToCartResult.ProductVariantId, quantity = addToCartResult.Quantity });

            // else just go to cart
            return _uniquePageService.RedirectTo<Pages.Cart>();
        }

        public AddToCartResult Add(AddToCartModel addToCartModel)
        {
            if (_cartValidationService.CanAddQuantity(addToCartModel).Valid)
            {
                _cartItemManager.AddToCart(addToCartModel);
                return new AddToCartResult
                {
                    Success = true,
                    ProductVariantId = addToCartModel.ProductVariant.Id,
                    Quantity = addToCartModel.Quantity
                };
            }
            return new AddToCartResult();
        }
    }
}