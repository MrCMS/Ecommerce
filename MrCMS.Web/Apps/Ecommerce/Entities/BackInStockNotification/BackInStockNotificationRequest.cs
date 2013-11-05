using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification
{
    public class BackInStockNotificationRequest : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsNotified { get; set; }
    }
}