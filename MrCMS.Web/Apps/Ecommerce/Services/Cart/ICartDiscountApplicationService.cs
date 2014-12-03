using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartDiscountApplicationService
    {
        CheckLimitationsResult CheckLimitations(Discount discount, CartModel cart);
        DiscountApplicationInfo ApplyDiscount(DiscountInfo discountInfo, CartModel cart);
    }
}