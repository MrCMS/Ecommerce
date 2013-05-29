using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationValue : SiteEntity
    {
        public virtual ProductSpecificationAttribute Option { get; set; }
        public virtual string Value { get; set; }
        public virtual Product Product { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}