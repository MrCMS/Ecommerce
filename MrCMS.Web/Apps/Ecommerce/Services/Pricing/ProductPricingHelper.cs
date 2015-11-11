using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public static class ProductPricingHelper
    {
        public static CartItemData GetCartItemDataFromProductVariant(this ProductVariant productVariant, IProductPricingMethod pricing, int quantity = 1)
        {
            return new CartItemData { Item = productVariant, Quantity = quantity, Pricing = pricing };
        }
    }
}