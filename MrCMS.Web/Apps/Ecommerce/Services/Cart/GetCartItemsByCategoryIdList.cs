using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetCartItemsByCategoryIdList : IGetCartItemsByCategoryIdList
    {
        public List<CartItemData> GetCartItems(CartModel cart, string categoryIds)
        {
            List<int> categories = (categoryIds ?? string.Empty).GetIntList();

            return cart.Items.FindAll(x =>
            {
                var itemCategories = x.Item.Product.Categories.Select(c => c.Id);
                return categories.Intersect(itemCategories).Any();
            });
        }
    }
}