using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductStockChecker
    {
        bool IsInStock(ProductVariant productVariant);
        bool CanOrderQuantity(ProductVariant productVariant, int quantity);
    }
}