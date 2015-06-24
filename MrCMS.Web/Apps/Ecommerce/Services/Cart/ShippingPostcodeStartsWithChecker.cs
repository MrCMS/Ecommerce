using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ShippingPostcodeStartsWithChecker : DiscountLimitationChecker<ShippingPostcodeStartsWith>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public ShippingPostcodeStartsWithChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(ShippingPostcodeStartsWith limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            if (cart.ShippingAddress == null)
                return
                    CheckLimitationsResult.CurrentlyInvalid(_stringResourceProvider.GetValue("Shipping address is not yet set"));
            var postcodes =
                (limitation.ShippingPostcode ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToUpper())
                    .ToHashSet();
            var postcode = cart.ShippingAddress.FormattedPostcode();
            return postcodes.Any(postcode.StartsWith)
                ? CheckLimitationsResult.Successful(Enumerable.Empty<CartItem>())
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue("Shipping postcode is not valid for this discount"));

        }
    }
}