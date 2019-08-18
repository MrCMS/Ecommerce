using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ItemBrandIsNotChecker : DiscountLimitationChecker<ItemBrandIsNot>
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetCartItemsBySKUList _getCartItemsBySKUList;

        public ItemBrandIsNotChecker(IStringResourceProvider stringResourceProvider, IGetCartItemsBySKUList getCartItemsBySKUList)
        {
            _stringResourceProvider = stringResourceProvider;
            _getCartItemsBySKUList = getCartItemsBySKUList;
        }

        public override CheckLimitationsResult CheckLimitations(ItemBrandIsNot limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            var cartItems = _getCartItemsBySKUList.GetNonExcudedBrandCartItems(cart, limitation.Brands);

            return cartItems.Any()
                ? CheckLimitationsResult.Successful(cartItems)
                : CheckLimitationsResult.NeverValid("");
        }
    }
}