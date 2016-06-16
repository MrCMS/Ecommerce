using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class OnlineCustomerCartItem
    {
        public ProductVariant Product { get; set; }
        public int Quantity { get; set; }
    }
}