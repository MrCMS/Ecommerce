using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class RelatedProductsViewModel
    {
        public RelatedProductsViewModel()
        {
            Products = new List<ProductCardModel>();
        }

        public IList<ProductCardModel> Products { get; set; }
        public string Title { get; set; }
        public CartModel Cart { get; set; }
    }
}