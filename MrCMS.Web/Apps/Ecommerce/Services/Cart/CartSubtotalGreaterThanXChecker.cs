using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartSubtotalGreaterThanXChecker : DiscountLimitationChecker<CartSubtotalGreaterThanX>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public CartSubtotalGreaterThanXChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(CartSubtotalGreaterThanX limitation, CartModel cart)
        {
            return cart.Subtotal >= limitation.CartSubtotalGreaterThanValue
                ? CheckLimitationsResult.Successful(Enumerable.Empty<CartItem>())
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue("Order total does not reach the threshold of " +
                                                     limitation.CartSubtotalGreaterThanValue.ToCurrencyFormat()));
        }
    }
}