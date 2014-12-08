using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetCartItemsByCategoryIdList
    {
        List<CartItem> GetCartItems(CartModel cart, string categoryIds);
    }
}