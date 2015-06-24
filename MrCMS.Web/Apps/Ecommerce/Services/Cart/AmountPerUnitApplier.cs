using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AmountPerUnitApplier : CartItemBasedDiscountApplicationApplier<AmountPerUnit>
    {
        public AmountPerUnitApplier(IGetCartItemBasedApplicationProducts getCartItemBasedApplicationProducts)
            : base(getCartItemBasedApplicationProducts)
        {
        }

        public override DiscountApplicationInfo Apply(AmountPerUnit application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            var cartItems = GetItems(application, checkLimitationsResult, cart);
            return new DiscountApplicationInfo
            {
                ItemDiscounts =
                    cartItems.ToDictionary(cartItem => cartItem.Id,
                        item => application.DiscountAmount * item.Quantity)
            };
        }
    }
}