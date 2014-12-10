using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public abstract class CartItemBasedDiscountApplication : DiscountApplication
    {
        protected CartItemBasedDiscountApplication()
        {
            CartItemsFromLimitations = true;
        }
        [DisplayName("Use Cart Items From Limitations?")]
        public virtual bool CartItemsFromLimitations { get; set; }

        [DisplayName("SKUs (comma delimited list)")]
        public virtual string SKUs { get; set; }

        [DisplayName("Category IDs (comma delimited list)")]
        public virtual string CategoryIds { get; set; }
    }
}