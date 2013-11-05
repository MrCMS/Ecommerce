using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification
{
    public class BackInStockProductVariant : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual bool Processed { get; set; }
    }
}