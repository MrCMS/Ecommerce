using System.Collections.Generic;
using System.IO;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate
{
    public interface IBulkShippingUpdateValidationService
    {
        Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<BulkShippingUpdateDataTransferObject> items);
        List<BulkShippingUpdateDataTransferObject> ValidateAndBulkShippingUpdateOrders(Stream file, ref Dictionary<string, List<string>> parseErrors);
    }
}