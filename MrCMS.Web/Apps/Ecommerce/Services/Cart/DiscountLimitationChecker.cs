using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public abstract class DiscountLimitationChecker
    {
        public abstract CheckLimitationsResult CheckLimitations(DiscountLimitation limitation, CartModel cart);
    }

    public abstract class DiscountLimitationChecker<T> : DiscountLimitationChecker where T : DiscountLimitation
    {
        public abstract CheckLimitationsResult CheckLimitations(T limitation, CartModel cart);

        public override sealed CheckLimitationsResult CheckLimitations(DiscountLimitation limitation, CartModel cart)
        {
            var typedLimitation = limitation as T;
            return typedLimitation != null
                ? CheckLimitations(typedLimitation, cart)
                : CheckLimitationsResult.Failure("Passed limitation is of invalid type");
        }
    }
}