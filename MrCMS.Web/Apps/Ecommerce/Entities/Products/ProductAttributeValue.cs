using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductAttributeValue : SiteEntity
    {
        public virtual ProductAttributeOption ProductAttributeOption { get; set; }
        public virtual string Value { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}