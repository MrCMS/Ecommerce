using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartItemManager
    {
        void AddToCart(AddToCartModel model);
        void Delete(CartItem item);
        void UpdateQuantities(List<CartUpdateValue> quantities);
    }
}