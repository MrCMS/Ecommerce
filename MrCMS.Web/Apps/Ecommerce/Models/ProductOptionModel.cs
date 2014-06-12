using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductOptionModel<T>
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<ProductValueModel<T>> Values { get; set; }
    }
}