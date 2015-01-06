using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class CategoryProductDisplayOrder : SiteEntity
    {
        public virtual Product Product { get; set; }
        public virtual Category Category { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}