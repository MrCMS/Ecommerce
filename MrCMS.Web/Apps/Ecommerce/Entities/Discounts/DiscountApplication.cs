using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public abstract class DiscountApplication : SiteEntity
    {
        public DiscountApplication()
        {
            Discounts = new List<Discount>();
        }

        public abstract decimal GetDiscount(CartModel cartModel);
        public abstract decimal GetDiscount(CartItem cartItem);

        public virtual IList<Discount> Discounts { get; set; }

        public abstract void CopyValues(DiscountApplication application);
    }
}