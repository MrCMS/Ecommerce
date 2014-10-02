using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.Rules
{
    public interface IBulkStockUpdateValidationRule
    {
        IEnumerable<string> GetErrors(IEnumerable<BulkStockUpdateDataTransferObject> items);
    }
}