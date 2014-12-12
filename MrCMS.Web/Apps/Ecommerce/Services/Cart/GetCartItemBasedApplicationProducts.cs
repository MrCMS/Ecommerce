using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartItemBasedApplicationProducts : IGetCartItemBasedApplicationProducts
    {
        private readonly IGetCartItemsBySKUList _getCartItemsBySKUList;
        private readonly IGetCartItemsByCategoryIdList _getCartItemsByCategoryIdList;

        public GetCartItemBasedApplicationProducts(IGetCartItemsBySKUList getCartItemsBySKUList,IGetCartItemsByCategoryIdList getCartItemsByCategoryIdList)
        {
            _getCartItemsBySKUList = getCartItemsBySKUList;
            _getCartItemsByCategoryIdList = getCartItemsByCategoryIdList;
        }

        public HashSet<CartItem> Get(CartItemBasedDiscountApplication application, CartModel cart)
        {
            var cartItems = new HashSet<CartItem>();
            cartItems.AddRange(_getCartItemsBySKUList.GetCartItems(cart, application.SKUs));
            cartItems.AddRange(_getCartItemsByCategoryIdList.GetCartItems(cart, application.SKUs));
            return cartItems;
        }
    }
}