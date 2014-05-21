using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class XPercent : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            if (cartModel.TotalPreDiscount > 0 && DiscountPercent > 0 && DiscountPercent <= 100)
                return Math.Round(cartModel.TotalPreDiscount*(DiscountPercent/100), 2, MidpointRounding.AwayFromZero);
            return decimal.Zero;
        }

        public override decimal GetDiscount(CartItem cartItem)
        {
            if (cartItem.PricePreDiscount > 0 && DiscountPercent > 0 && DiscountPercent <= 100)
                return Math.Round(cartItem.PricePreDiscount*(DiscountPercent/100), 2, MidpointRounding.AwayFromZero);
            return decimal.Zero;
        }

        [DisplayName("Discount % (e.g: 15)")]
        [Range(0, 100)]
        public virtual decimal DiscountPercent { get; set; }

        public override void CopyValues(DiscountApplication application)
        {
            this.DiscountPercent = ((XPercent)application).DiscountPercent;
        }
    }
}