using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IPerformSimpleBulkUpdate
    {
        BulkUpdateResult Update(IEnumerable<BulkStockUpdateDataTransferObject> items);
    }
}