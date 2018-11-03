using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetCartItemsBySKUList
    {
        List<CartItemData> GetCartItems(CartModel cart, string skuList);
        List<CartItemData> GetNonExcudedCartItems(CartModel cart, string skuList);
        IEnumerable<CartItemData> GetNonExcudedBrandCartItems(CartModel cart, string limitationBrands);
    }
}