using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductOptionValueSort : SiteEntity
    {
        public virtual ProductOption ProductOption { get; set; }
        public virtual string Value { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}