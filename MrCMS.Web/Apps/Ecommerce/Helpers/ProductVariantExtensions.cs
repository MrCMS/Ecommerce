using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductVariantExtensions
    {
        public static int GetStockRemaining(this ProductVariant variant)
        {
            return MrCMSApplication.Get<IGetStockRemainingQuantity>().Get(variant);
        }
    }
}