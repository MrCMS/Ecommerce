using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase
{
    public class GoogleBaseProduct : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual string Category { get; set; }
    }
}