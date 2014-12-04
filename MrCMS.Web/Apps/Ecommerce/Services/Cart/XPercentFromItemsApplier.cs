using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class XPercentFromItemsApplier : CartItemBasedDiscountApplicationApplier<XPercentFromItems>
    {
        public XPercentFromItemsApplier(IGetCartItemBasedApplicationProducts getCartItemBasedApplicationProducts)
            : base(getCartItemBasedApplicationProducts)
        {
        }

        public override DiscountApplicationInfo Apply(XPercentFromItems application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            var cartItems = GetItems(application, checkLimitationsResult, cart);
            return new DiscountApplicationInfo
            {
                ItemDiscounts =
                    cartItems.ToDictionary(cartItem => cartItem.Id,
                        item => item.PricePreDiscount*(application.DiscountPercent/100m))
            };
        }
    }
}