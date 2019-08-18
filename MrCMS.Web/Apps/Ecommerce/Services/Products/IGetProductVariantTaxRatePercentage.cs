using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IGetProductVariantTaxRatePercentage
    {
        decimal GetTaxRatePercentage(ProductVariant productVariant);
    }

    public class GetProductVariantTaxRatePercentage : IGetProductVariantTaxRatePercentage
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly TaxSettings _taxSettings;

        public GetProductVariantTaxRatePercentage(IGetDefaultTaxRate getDefaultTaxRate, TaxSettings taxSettings)
        {
            _getDefaultTaxRate = getDefaultTaxRate;
            _taxSettings = taxSettings;
        }

        public decimal GetTaxRatePercentage(ProductVariant productVariant)
        {
            TaxRate defaultTaxRate = _getDefaultTaxRate.Get();
            var taxRate = productVariant == null ? null : productVariant.TaxRate;
            return _taxSettings.TaxesEnabled
                ? taxRate == null
                    ? defaultTaxRate != null
                        ? defaultTaxRate.Percentage
                        : decimal.Zero
                    : taxRate.Percentage
                : decimal.Zero;
        }
    }
}