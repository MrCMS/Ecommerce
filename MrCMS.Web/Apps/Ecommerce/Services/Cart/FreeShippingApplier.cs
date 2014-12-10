using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class FreeShippingApplier : DiscountApplicationApplier<FreeShipping>
    {
        public override DiscountApplicationInfo Apply(FreeShipping application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            return new DiscountApplicationInfo
            {
                ShippingDiscount = cart.ShippingTotalPreDiscount
            };
        }
    }
}