using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class CartHasAtLeastXItems : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return string.Format("Cart has at least {0} item(s)", NumberOfItems); }
        }

        [DisplayName("Number of items")]
        public virtual int NumberOfItems { get; set; }
    }
}