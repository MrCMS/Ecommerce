using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationOption : SiteEntity
    {
        public ProductSpecificationOption()
        {
            Values = new List<ProductSpecificationValue>();
        }
        public virtual string Name { get; set; }

        public virtual IList<ProductSpecificationValue> Values { get; set; }
    }
}