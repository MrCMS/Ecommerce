using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public abstract class DiscountLimitation : SiteEntity
    {
        public abstract string DisplayName { get; }
        public virtual Discount Discount { get; set; }
    }
}