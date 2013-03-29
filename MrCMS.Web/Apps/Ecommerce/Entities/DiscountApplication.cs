using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public abstract class DiscountApplication : SiteEntity
    {
        public abstract decimal GetDiscount(CartModel cartModel);
    }

    class FixedAmount : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            return DiscountAmount;
        }

        public virtual decimal DiscountAmount { get; set; }
    }
}