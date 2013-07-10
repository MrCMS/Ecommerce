using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class TaxRateIdIsValid : IProductVariantImportValidationRule
    {
        private readonly ITaxRateManager _taxRateManager;

        public TaxRateIdIsValid(ITaxRateManager taxRateManager)
        {
            _taxRateManager = taxRateManager;
        }

        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            if (productVariant.TaxRate.HasValue && _taxRateManager.Get(productVariant.TaxRate.Value) == null)
                yield return "The tax rate Id specified is not present within the system.";
        }
    }
}