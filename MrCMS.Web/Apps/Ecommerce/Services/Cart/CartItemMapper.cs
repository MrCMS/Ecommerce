using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public static class CartItemMapper
    {
        public static CartItemData GetCartItemData(this CartItem cartItem)
        {
            return new CartItemData
            {
                Id = cartItem.Id,
                Data = cartItem.Data,
                Item = cartItem.Item,
                Quantity = cartItem.Quantity,
                UserGuid = cartItem.UserGuid
            };
        }
    }
}