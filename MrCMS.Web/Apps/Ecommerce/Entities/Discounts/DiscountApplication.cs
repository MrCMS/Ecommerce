using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public abstract class DiscountApplication : SiteEntity
    {
        public virtual Discount Discount { get; set; }

        public abstract string DisplayName { get; }
    }
}