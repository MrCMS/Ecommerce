using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ProductAttributeValue : SiteEntity
    {
        public virtual ProductAttributeOption Option { get; set; }
        public virtual string Value { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}