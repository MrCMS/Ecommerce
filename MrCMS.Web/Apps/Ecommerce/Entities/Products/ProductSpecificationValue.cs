using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.ComponentModel.DataAnnotations;
namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationValue : SiteEntity
    {
        public virtual ProductSpecificationAttribute ProductSpecificationAttribute { get; set; }
        public virtual string Value { get; set; }
        public virtual Product Product { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}