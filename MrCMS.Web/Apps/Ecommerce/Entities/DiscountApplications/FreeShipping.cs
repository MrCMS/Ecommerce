using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class FreeShipping : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            return cartModel.ShippingTotal;
        }

        public override decimal GetDiscount(CartItem cartItem)
        {
            return decimal.Zero;
        }

        public override void CopyValues(DiscountApplication application)
        {
        }
    }
}