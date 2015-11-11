using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class OnlyByItselfChecker : DiscountLimitationChecker<OnlyByItself>
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public OnlyByItselfChecker(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(OnlyByItself limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            var discount = limitation.Discount;
            if (discount == null)
                return CheckLimitationsResult.NeverValid(
                        _stringResourceProvider.GetValue("This limitation has no discount."));

            if (allDiscounts.Any(x => x != discount))
                return CheckLimitationsResult.CurrentlyInvalid(
                    _stringResourceProvider.GetValue("This discount cannot be combined."));

            return CheckLimitationsResult.Successful(Enumerable.Empty<CartItemData>());
        }

    }
}