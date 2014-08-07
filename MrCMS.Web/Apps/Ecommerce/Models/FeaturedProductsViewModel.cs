using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class FeaturedProductsViewModel
    {
        public FeaturedProductsViewModel()
        {
            Products = new List<ProductCardModel>();
        }
        public List<ProductCardModel> Products { get; set; }
        public string Title { get; set; }
        public CartModel Cart { get; set; }
    }
}