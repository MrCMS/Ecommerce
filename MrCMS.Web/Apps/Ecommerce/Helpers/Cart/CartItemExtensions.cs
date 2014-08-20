using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Cart
{
    public static class CartItemExtensions
    {
        public static bool IsAbleToUseShippingMethod(this CartItem item, IShippingMethod shippingMethod)
        {
            var productVariant = item.Item;
            if (!productVariant.RequiresShipping)
                return true;
            return !productVariant.HasRestrictedShipping ||
                   productVariant.RestrictedTo.Contains(shippingMethod.TypeName);
        }
    }
}