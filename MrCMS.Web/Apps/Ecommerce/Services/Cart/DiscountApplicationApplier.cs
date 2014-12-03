using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public abstract class DiscountApplicationApplier
    {
        public abstract DiscountApplicationInfo Apply(DiscountApplication application, CartModel cart, CheckLimitationsResult checkLimitationsResult);
    }

    public abstract class DiscountApplicationApplier<T> : DiscountApplicationApplier where T : DiscountApplication
    {
        public abstract DiscountApplicationInfo Apply(T application, CartModel cart);

        public override sealed DiscountApplicationInfo Apply(DiscountApplication application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            var typedApplication = application as T;
            return typedApplication != null
                ? Apply(typedApplication, cart)
                : new DiscountApplicationInfo();
        }
    }
}