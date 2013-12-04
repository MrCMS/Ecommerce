using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartItemBuilder
    {
        private int _quantity = 1;

        public CartItem Build()
        {
            return new CartItem
                       {
                           Quantity = _quantity,
                           Item = new ProductVariant
                                      {
                                      }
                       };
        }
    }
}