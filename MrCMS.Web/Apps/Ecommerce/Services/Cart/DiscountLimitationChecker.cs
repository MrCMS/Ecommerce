using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public abstract class DiscountLimitationChecker
    {
        public abstract CheckLimitationsResult CheckLimitations(DiscountLimitation limitation, CartModel cart, IList<Discount> allDiscounts);
    }

    public abstract class DiscountLimitationChecker<T> : DiscountLimitationChecker where T : DiscountLimitation
    {
        public abstract CheckLimitationsResult CheckLimitations(T limitation, CartModel cart, IList<Discount> allDiscounts);

        public override sealed CheckLimitationsResult CheckLimitations(DiscountLimitation limitation, CartModel cart, IList<Discount> allDiscounts)
        {
            var typedLimitation = limitation as T;
            return typedLimitation != null
                ? CheckLimitations(typedLimitation, cart, allDiscounts)
                : CheckLimitationsResult.CurrentlyInvalid("Passed limitation is of invalid type");
        }
    }
}