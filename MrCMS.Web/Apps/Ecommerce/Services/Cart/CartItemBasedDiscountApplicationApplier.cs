using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public abstract class CartItemBasedDiscountApplicationApplier<T> : DiscountApplicationApplier<T>
        where T : CartItemBasedDiscountApplication
    {
        private readonly IGetCartItemBasedApplicationProducts _getCartItemBasedApplicationProducts;


        protected CartItemBasedDiscountApplicationApplier(IGetCartItemBasedApplicationProducts getCartItemBasedApplicationProducts)
        {
            _getCartItemBasedApplicationProducts = getCartItemBasedApplicationProducts;
        }

        protected HashSet<CartItemData> GetItems(T application, CheckLimitationsResult checkLimitationsResult, CartModel cart)
        {
            if (application.CartItemsFromLimitations)
            {
                return checkLimitationsResult.CartItems;
            }
            return _getCartItemBasedApplicationProducts.Get(application, cart);

        }
    }
}