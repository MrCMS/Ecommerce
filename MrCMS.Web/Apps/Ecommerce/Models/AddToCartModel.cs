using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class AddToCartModel
    {
        public ProductVariant ProductVariant { get; set; }

        [Remote("CanAddQuantity", "Cart", AdditionalFields = "ProductVariant.Id")]
        public int Quantity { get; set; }
    }
}