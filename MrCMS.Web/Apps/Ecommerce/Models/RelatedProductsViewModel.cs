using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class RelatedProductsViewModel
    {
        public IList<Product> Products { get; set; }
        public string Title { get; set; }
        public CartModel Cart { get; set; }
    }
}