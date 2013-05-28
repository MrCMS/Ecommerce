using System.Collections.Generic;
using MrCMS.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationAttributeOption : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual ProductSpecificationAttribute ProductSpecificationAttribute { get; set; }
    }
}