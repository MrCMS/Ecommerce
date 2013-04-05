using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public abstract class DiscountApplication : SiteEntity
    {
        public DiscountApplication()
        {
            Discounts = new List<Discount>();
        }

        public abstract decimal GetDiscount(CartModel cartModel);

        public virtual IList<Discount> Discounts { get; set; }

        public abstract void CopyValues(DiscountApplication application);
    }

    public class FixedAmount : DiscountApplication
    {
        public override decimal GetDiscount(CartModel cartModel)
        {
            return DiscountAmount;
        }

        public virtual decimal DiscountAmount { get; set; }

        public override void CopyValues(DiscountApplication application)
        {
            this.DiscountAmount = ((FixedAmount)application).DiscountAmount;
        }
    }
}