using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class OrderTotalFixedAmountApplier : DiscountApplicationApplier<OrderTotalFixedAmount>
    {
        public override DiscountApplicationInfo Apply(OrderTotalFixedAmount application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            return new DiscountApplicationInfo
            {
                OrderDiscount = application.DiscountAmount
            };
        }
    }
}