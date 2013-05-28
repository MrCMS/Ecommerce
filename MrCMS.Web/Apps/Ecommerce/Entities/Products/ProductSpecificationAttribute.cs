using System.Collections.Generic;
using MrCMS.Entities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationAttribute : SiteEntity
    {
        public ProductSpecificationAttribute()
        {
            Values = new List<ProductSpecificationValue>();
            Options = new List<ProductSpecificationAttributeOption>();
        }

        [Required]
        [Remote("IsUniqueAttribute", "ProductSpecificationAttribute", AdditionalFields = "Id")]
        public virtual string Name { get; set; }

        public virtual IList<ProductSpecificationValue> Values { get; set; }
        public virtual IList<ProductSpecificationAttributeOption> Options { get; set; }
    }
}