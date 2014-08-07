using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductVariantAvailabilityService
    {
        CanBuyStatus CanBuy(ProductVariant productVariant, int additionalQuantity = 0);
    }
}