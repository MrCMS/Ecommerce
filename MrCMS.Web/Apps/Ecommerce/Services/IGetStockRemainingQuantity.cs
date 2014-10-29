using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetStockRemainingQuantity
    {
        int Get(ProductVariant productVariant);
    }
}