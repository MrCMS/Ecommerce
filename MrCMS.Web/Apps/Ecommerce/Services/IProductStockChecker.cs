using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductStockChecker
    {
        bool IsInStock(ProductVariant productVariant);
        CanOrderQuantityResult CanOrderQuantity(ProductVariant productVariant, int quantity);
    }
}