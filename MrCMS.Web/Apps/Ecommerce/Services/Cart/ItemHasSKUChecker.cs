using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ItemHasSKUChecker : DiscountLimitationChecker<ItemHasSKU>
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetCartItemsBySKUList _getCartItemsBySKUList;

        public ItemHasSKUChecker(IStringResourceProvider stringResourceProvider, IGetCartItemsBySKUList getCartItemsBySKUList)
        {
            _stringResourceProvider = stringResourceProvider;
            _getCartItemsBySKUList = getCartItemsBySKUList;
        }

        public override CheckLimitationsResult CheckLimitations(ItemHasSKU limitation, CartModel cart)
        {
            var cartItems = _getCartItemsBySKUList.GetCartItems(cart, limitation.SKUs);

            return cartItems.Any()
                ? CheckLimitationsResult.Successful(cartItems)
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue(
                        "You don't have the required item(s) in your cart for this discount"));
        }
    }
}