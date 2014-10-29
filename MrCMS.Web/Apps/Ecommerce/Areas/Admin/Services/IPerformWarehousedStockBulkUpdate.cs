using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IPerformWarehousedStockBulkUpdate
    {
        BulkUpdateResult Update(List<BulkWarehouseStockUpdateDTO> dtoList);
    }
}