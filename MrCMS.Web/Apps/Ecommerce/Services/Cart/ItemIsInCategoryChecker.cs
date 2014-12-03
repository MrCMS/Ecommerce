using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ItemIsInCategoryChecker : DiscountLimitationChecker<ItemIsInCategory>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public ItemIsInCategoryChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(ItemIsInCategory limitation, CartModel cart)
        {
            var categories = (limitation.CategoryIds ?? string.Empty).GetIntList();

            List<CartItem> cartItems = cart.Items.FindAll(x => x.Item.Product.Categories.Select(c => c.Id).Any(categories.Contains));

            return cartItems.Any()
                ? CheckLimitationsResult.Successful(cartItems)
                : CheckLimitationsResult.Failure(
                    _stringResourceProvider.GetValue(
                        "You don't have the required item(s) in your cart for this discount"));
        }
    }
}