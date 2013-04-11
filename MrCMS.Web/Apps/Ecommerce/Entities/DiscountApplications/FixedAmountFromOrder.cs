using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class FixedAmountFromOrder : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            return DiscountAmount;
        }

        public override decimal GetDiscount(CartItem cartItem)
        {
            return decimal.Zero;
        }

        public virtual decimal DiscountAmount { get; set; }

        public override void CopyValues(DiscountApplication application)
        {
            this.DiscountAmount = ((FixedAmountFromOrder)application).DiscountAmount;
        }
    }
}