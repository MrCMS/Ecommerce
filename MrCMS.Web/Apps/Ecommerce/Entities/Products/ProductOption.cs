using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductOption : SiteEntity
    {
        public ProductOption()
        {
            Products = new List<Product>();
            Values = new List<ProductOptionValue>();
        }
        [Required]
        public virtual string Name { get; set; }
        public virtual IList<Product> Products { get; set; }
        public virtual IList<ProductOptionValue> Values { get; set; }
    }
}