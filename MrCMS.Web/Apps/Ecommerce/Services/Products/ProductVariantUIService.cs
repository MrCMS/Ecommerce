using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductVariantUIService : IProductVariantUIService
    {
        private readonly IProductVariantAvailabilityService _productVariantAvailabilityService;

        public ProductVariantUIService(IProductVariantAvailabilityService productVariantAvailabilityService)
        {
            _productVariantAvailabilityService = productVariantAvailabilityService;
        }

        public CanBuyStatus CanBuyAny(ProductVariant productVariant)
        {
            return _productVariantAvailabilityService.CanBuy(productVariant, 1);
        }
    }
}