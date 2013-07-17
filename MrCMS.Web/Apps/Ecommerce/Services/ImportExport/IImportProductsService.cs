using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsService
    {
        void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport);
    }
}