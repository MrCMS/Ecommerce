using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ItemHasSKUChecker : DiscountLimitationChecker<ItemHasSKU>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public ItemHasSKUChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(ItemHasSKU limitation, CartModel cart)
        {
            HashSet<string> skus =
                (limitation.SKUs ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToHashSet();

            List<CartItem> cartItems =
                cart.Items.FindAll(x => skus.Contains(x.Item.SKU, StringComparer.InvariantCultureIgnoreCase));

            return cartItems.Any()
                ? CheckLimitationsResult.Successful(cartItems)
                : CheckLimitationsResult.Failure(
                    _stringResourceProvider.GetValue(
                        "You don't have the required item(s) in your cart for this discount"));
        }
    }
}