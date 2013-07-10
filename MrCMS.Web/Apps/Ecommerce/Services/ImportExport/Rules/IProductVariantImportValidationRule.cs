using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public interface IProductVariantImportValidationRule
    {
        IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant);
    }
}