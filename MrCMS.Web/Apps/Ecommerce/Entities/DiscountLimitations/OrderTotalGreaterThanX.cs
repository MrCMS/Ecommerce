using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class CartSubtotalGreaterThanX : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return "Cart product subtotal is greater than x"; }
        }

        public override bool IsCartValid(CartModel cartModel)
        {
            return cartModel.TotalPreDiscount > OrderTotalGreaterThanValue;
        }

        public override bool IsItemValid(CartItem cartItem)
        {
            return false;
        }

        [DisplayName("Value")]
        public virtual decimal OrderTotalGreaterThanValue { get; set; }

        public override void CopyValues(DiscountLimitation limitation)
        {
            this.OrderTotalGreaterThanValue = ((CartSubtotalGreaterThanX)limitation).OrderTotalGreaterThanValue;
        }
    }
}