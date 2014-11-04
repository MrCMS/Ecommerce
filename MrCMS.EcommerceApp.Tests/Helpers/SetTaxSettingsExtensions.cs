using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Ninject;

namespace MrCMS.EcommerceApp.Tests.Helpers
{
    public static class SetTaxSettingsExtensions
    {
        public static void SetTaxSettings(this IKernel kernel, bool taxesEnabled = false, bool loadedPricesIncludeTax = false, bool shippingTaxesEnabled = false, bool shippingPricesIncludeTax = false)
        {
            var taxSettings = new TaxSettings
            {
                TaxesEnabled = taxesEnabled,
                LoadedPricesIncludeTax = loadedPricesIncludeTax,
                ShippingRateTaxesEnabled = shippingTaxesEnabled,
                ShippingRateIncludesTax = shippingPricesIncludeTax
            };
            kernel.Rebind<TaxSettings>().ToConstant(taxSettings);
            kernel.Rebind<IProductPricingService>().ToConstant(new ProductPricingService(taxSettings));
        }
    }
}