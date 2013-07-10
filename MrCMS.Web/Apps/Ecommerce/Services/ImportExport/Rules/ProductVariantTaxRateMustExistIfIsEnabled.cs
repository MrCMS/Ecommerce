using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantTaxRateMustExistIfIsEnabled : IProductVariantImportValidationRule
    {
        private readonly TaxSettings _taxSettings;
        
        public ProductVariantTaxRateMustExistIfIsEnabled(TaxSettings taxSettings)
        {
            _taxSettings = taxSettings;
        }

        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            if (_taxSettings.TaxesEnabled && (!productVariant.TaxRate.HasValue || productVariant.TaxRate==0))
            {
                yield return "A tax rate is not set, but taxes are enabled within the system.";
            }
        }
    }
}