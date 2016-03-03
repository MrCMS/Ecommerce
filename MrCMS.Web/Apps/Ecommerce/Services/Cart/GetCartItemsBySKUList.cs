using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartItemsBySKUList : IGetCartItemsBySKUList
    {
        public List<CartItemData> GetCartItems(CartModel cart, string skuList)
        {
            HashSet<string> skus = (skuList ?? string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet();

            List<CartItemData> cartItems =
                cart.Items.FindAll(x => skus.Contains(x.Item.SKU, StringComparer.InvariantCultureIgnoreCase));
            return cartItems;
        }
    }
}