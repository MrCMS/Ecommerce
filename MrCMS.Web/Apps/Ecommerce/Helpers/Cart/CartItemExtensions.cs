using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Cart
{
    public static class CartItemExtensions
    {
        public static bool IsUnableToUseShippingMethod(this CartItem item, IShippingMethod shippingMethod)
        {
            var productVariant = item.Item;
            if (!productVariant.RequiresShipping)
                return false;
            if(!productVariant.HasRestrictedShipping)
                return false;
            return !productVariant.RestrictedTo.Contains(shippingMethod.TypeName);
        }
    }
}