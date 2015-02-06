using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IGetProductSearchSortByValue
    {
        ProductSearchSort Get(ProductSearchQuery query);
        bool CategoryHasBeenSorted(ProductSearchQuery query);
    }
}