using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    public interface IBulkShippingUpdateValidationRule
    {
        IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item);
    }
}