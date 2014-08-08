using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    public class ShippingMethodExists : IBulkShippingUpdateValidationRule
    {
        public IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item)
        {
            throw new NotImplementedException();
            //var shippingMethod = _shippingMethodManager.GetByName(item.ShippingMethod);
            //if (shippingMethod == null)
            //    yield return string.Format("Shipping method {0} does not exist in the system.", item.OrderId);
        }
    }
}