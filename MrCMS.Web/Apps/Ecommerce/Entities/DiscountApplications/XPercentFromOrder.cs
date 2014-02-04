using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class XPercentFromOrder : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            if (cartModel.TotalPreDiscount > 0 && DiscountPercent > 0 && DiscountPercent <= 100)
                return cartModel.TotalPreDiscount * (DiscountPercent / 100);
            return decimal.Zero;
        }

        public override decimal GetDiscount(CartItem cartItem)
        {
            return decimal.Zero;
        }

        [DisplayName("Discount % (e.g: 15)")]
        [Range(0,100)]
        public virtual decimal DiscountPercent { get; set; }

        public override void CopyValues(DiscountApplication application)
        {
            this.DiscountPercent = ((XPercentFromOrder)application).DiscountPercent;
        }
    }
}