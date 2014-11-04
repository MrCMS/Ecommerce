using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IPeopleWhoBoughtThisService
    {
        PeopleWhoBoughtThisAlsoBoughtViewModel GetAlsoBoughtViewModel(Product product);
        PeopleWhoBoughtThisAlsoBoughtViewModel GetAlsoBoughtViewModel(ProductVariant variant);
    }
}