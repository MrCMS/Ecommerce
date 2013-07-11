using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantStockIsValid : IProductVariantImportValidationRule
    {
        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            if (productVariant.TrackingPolicy == TrackingPolicy.Track)
            {
                if (productVariant.Stock == 0)
                   yield return ("Variant Stock must have value if Tracking Policy is set to 'Track'.");
            }
        }
    }
}