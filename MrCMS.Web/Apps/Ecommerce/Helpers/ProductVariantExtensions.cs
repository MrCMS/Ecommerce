using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductVariantExtensions
    {
        public static int GetStockRemaining(this ProductVariant variant)
        {
            return MrCMSApplication.Get<IGetStockRemainingQuantity>().Get(variant);
        }

        public static decimal GetTaxRatePercentage(this TaxRate taxRate)
        {
            return MrCMSApplication.Get<TaxSettings>().TaxesEnabled
                ? taxRate == null
                    ? Decimal.Zero
                    : taxRate.Percentage
                : Decimal.Zero;
        }
    }
}