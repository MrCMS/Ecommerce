using System;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class XPercentFromOrderApplier : DiscountApplicationApplier<XPercentFromOrder>
    {
        public override DiscountApplicationInfo Apply(XPercentFromOrder application, CartModel cart, CheckLimitationsResult checkLimitationsResult)
        {
            return new DiscountApplicationInfo
            {
                OrderTotalDiscount =
                    Math.Round(cart.TotalPreDiscount*(application.DiscountPercent/100m), 2,
                        MidpointRounding.AwayFromZero)
            };
        }
    }
}