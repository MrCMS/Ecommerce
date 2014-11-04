using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Shipping
{
    public static class CartModelShippingExtensions
    {
        public static IEnumerable<CartItem> ShippableItems(this CartModel cartModel)
        {
            return cartModel.Items.Where(item => item.RequiresShipping);
        }

        public static decimal ShippableTotalPreDiscount(this CartModel cartModel)
        {
            return cartModel.ShippableItems().Sum(item => item.Price);
        }

        public static decimal ShippableCalculationTotal(this CartModel cartModel)
        {
            return cartModel.ShippableTotalPreDiscount() - cartModel.OrderTotalDiscount;
        }
    }
}