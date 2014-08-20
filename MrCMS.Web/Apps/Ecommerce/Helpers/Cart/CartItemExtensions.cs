using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Newtonsoft.Json;

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

        public static string GiftCardRecipient(this CartItem item)
        {
            if (item == null || item.Item == null || !item.Item.IsGiftCard)
                return null;

            var giftCardInfo = JsonConvert.DeserializeObject<GiftCardInfo>(item.Data);
            if (giftCardInfo == null)
                return null;
            return giftCardInfo.RecipientName;
        }
    }
}