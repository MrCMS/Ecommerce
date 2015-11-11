using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartTotalGreaterThanXChecker : DiscountLimitationChecker<CartTotalGreaterThanX>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public CartTotalGreaterThanXChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(CartTotalGreaterThanX limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            return cart.TotalPreDiscount > limitation.CartTotalGreaterThanValue
                ? CheckLimitationsResult.Successful(Enumerable.Empty<CartItemData>())
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue("Order total does not reach the threshold of " +
                                                     limitation.CartTotalGreaterThanValue.ToCurrencyFormat()));
        }
    }
}