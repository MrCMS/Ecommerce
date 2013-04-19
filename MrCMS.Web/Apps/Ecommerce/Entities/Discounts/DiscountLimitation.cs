using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public abstract class DiscountLimitation : SiteEntity
    {
        public DiscountLimitation()
        {
            Discounts = new List<Discount>();
        }

        public abstract string DisplayName { get; }
        public abstract bool IsCartValid(CartModel cartModel);
        public abstract bool IsItemValid(CartItem cartItem);
        public abstract void CopyValues(DiscountLimitation limitation);

        public virtual IList<Discount> Discounts { get; set; }
    }
}