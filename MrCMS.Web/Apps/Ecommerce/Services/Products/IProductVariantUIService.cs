using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductVariantUIService
    {
        CanBuyStatus CanBuyAny(ProductVariant productVariant);
    }
}