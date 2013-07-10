using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public interface IProductImportValidationRule
    {
        IEnumerable<string> GetErrors(ProductImportDataTransferObject product);
    }
}