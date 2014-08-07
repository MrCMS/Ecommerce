using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductAddedToCartUIService
    {
        CartModel Cart { get; }
        RedirectResult RedirectToCart();
        RelatedProductsViewModel GetRelatedProducts(ProductVariant productVariant);
        PeopleWhoBoughtThisAlsoBoughtViewModel CustomersAlsoBought(ProductVariant productVariant);
    }
}