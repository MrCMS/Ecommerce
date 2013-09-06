using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public interface IBulkShippingUpdateService
    {
        int BulkShippingUpdateFromDTOs(IEnumerable<BulkShippingUpdateDataTransferObject> items);
    }
}