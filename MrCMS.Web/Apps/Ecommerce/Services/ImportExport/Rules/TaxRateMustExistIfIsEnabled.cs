using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class TaxRateMustExistIfIsEnabled : IProductVariantImportValidationRule
    {
        private readonly TaxSettings _taxSettings;
        
        public TaxRateMustExistIfIsEnabled(TaxSettings taxSettings)
        {
            _taxSettings = taxSettings;
        }

        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            if (_taxSettings.TaxesEnabled && !productVariant.TaxRate.HasValue)
            {
                yield return "A tax rate is not set, but taxes are enabled within the system";
            }
        }
    }
}