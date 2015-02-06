using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public class DiscountUsage : SiteEntity
    {
        public virtual Discount Discount { get; set; }
        public virtual Order Order { get; set; }
    }
}