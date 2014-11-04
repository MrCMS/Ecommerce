using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Entities.GiftCards
{
    public class GiftCardUsage : SiteEntity
    {
        public virtual GiftCard GiftCard { get; set; }

        public virtual Order Order { get; set; }

        public virtual decimal Amount { get; set; }
    }
}