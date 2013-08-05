using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public interface IBulkStockUpdateService
    {
        int BulkStockUpdateFromDTOs(IEnumerable<BulkStockUpdateDataTransferObject> items);
    }
}