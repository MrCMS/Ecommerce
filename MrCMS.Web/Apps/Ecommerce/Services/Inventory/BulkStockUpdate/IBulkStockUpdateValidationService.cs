using System.Collections.Generic;
using System.IO;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public interface IBulkStockUpdateValidationService
    {
        Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<BulkStockUpdateDataTransferObject> items);
        List<BulkStockUpdateDataTransferObject> ValidateAndBulkStockUpdateProductVariants(Stream file, ref Dictionary<string, List<string>> parseErrors);
    }
}