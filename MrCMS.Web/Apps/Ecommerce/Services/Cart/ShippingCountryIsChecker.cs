using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ShippingCountryIsChecker : DiscountLimitationChecker<ShippingCountryIs>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public ShippingCountryIsChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(ShippingCountryIs limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            if (cart.ShippingAddress == null)
                return
                    CheckLimitationsResult.CurrentlyInvalid(_stringResourceProvider.GetValue("Shipping address is not yet set"));
            return cart.ShippingAddress.CountryCode == limitation.ShippingCountryCode
                ? CheckLimitationsResult.Successful(Enumerable.Empty<CartItem>())
                : CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue("Shipping country is not valid for this discount"));
        }
    }
}