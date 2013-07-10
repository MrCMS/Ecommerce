using System.Collections.Generic;
using MrCMS.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationAttributeOption : SiteEntity
    {
        public ProductSpecificationAttributeOption()
        {
            Values = new List<ProductSpecificationValue>();
        }

        [Required]
        [Remote("IsUniqueAttributeOption", "ProductSpecificationAttributeOption", AdditionalFields = "attributeId")]
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual ProductSpecificationAttribute ProductSpecificationAttribute { get; set; }

        public virtual IList<ProductSpecificationValue> Values { get; set; }
    }
}