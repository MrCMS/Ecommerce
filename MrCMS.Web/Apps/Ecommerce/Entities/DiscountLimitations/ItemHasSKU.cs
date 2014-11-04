using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemHasSKU : DiscountLimitation
    {
        public virtual string SKU { get; set; }
        public override string DisplayName
        {
            get { return "Item has SKU"; }
        }

        public override bool IsCartValid(CartModel cartModel)
        {
            return false;
        }

        public override bool IsItemValid(CartItem cartItem)
        {
            return cartItem.Item.SKU != null && SKU != null &&
                   cartItem.Item.SKU.Trim().Equals(SKU.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public override void CopyValues(DiscountLimitation limitation)
        {
            SKU = ((ItemHasSKU)limitation).SKU;
        }
    }
}