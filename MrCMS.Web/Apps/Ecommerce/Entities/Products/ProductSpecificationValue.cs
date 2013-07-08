using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationValue : SiteEntity
    {
        public virtual ProductSpecificationAttributeOption ProductSpecificationAttributeOption { get; set; }
        public virtual string Value { get { return ProductSpecificationAttributeOption == null ? string.Empty : ProductSpecificationAttributeOption.Name; } }
        public virtual Product Product { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual string SpecificationName
        {
            get
            {
                return ProductSpecificationAttributeOption != null &&
                       ProductSpecificationAttributeOption.ProductSpecificationAttribute != null
                           ? ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name
                           : string.Empty;
            }
        }
    }
}