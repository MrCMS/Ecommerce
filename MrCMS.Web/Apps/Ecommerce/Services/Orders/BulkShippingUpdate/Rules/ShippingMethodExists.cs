using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    public class ShippingMethodExists : IBulkShippingUpdateValidationRule
    {
        private readonly IShippingMethodManager _shippingMethodManager;

        public ShippingMethodExists(IShippingMethodManager shippingMethodManager)
        {
            _shippingMethodManager = shippingMethodManager;
        }

        public IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item)
        {
            var shippingMethod = _shippingMethodManager.GetByName(item.ShippingMethod);
            if (shippingMethod == null)
                yield return string.Format("Shipping method {0} does not exist in the system.", item.OrderId);
        }
    }
}