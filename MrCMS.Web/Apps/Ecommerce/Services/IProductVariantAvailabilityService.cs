using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductVariantAvailabilityService
    {
        CanBuyStatus CanBuy(ProductVariant productVariant, int quantity);
    }
}