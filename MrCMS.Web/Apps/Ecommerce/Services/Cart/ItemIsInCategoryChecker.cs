using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ItemIsInCategoryChecker : DiscountLimitationChecker<ItemIsInCategory>
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetCartItemsByCategoryIdList _getCartItemsByCategoryIdList;

        public ItemIsInCategoryChecker(IStringResourceProvider stringResourceProvider, IGetCartItemsByCategoryIdList getCartItemsByCategoryIdList)
        {
            _stringResourceProvider = stringResourceProvider;
            _getCartItemsByCategoryIdList = getCartItemsByCategoryIdList;
        }

        public override CheckLimitationsResult CheckLimitations(ItemIsInCategory limitation, CartModel cart)
        {
            var categoryIds = limitation.CategoryIds;
            var cartItems = _getCartItemsByCategoryIdList.GetCartItems(cart, categoryIds);

            return cartItems.Any()
                ? CheckLimitationsResult.Successful(cartItems)
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue(
                        "You don't have the required item(s) in your cart for this discount"));
        }
    }
}