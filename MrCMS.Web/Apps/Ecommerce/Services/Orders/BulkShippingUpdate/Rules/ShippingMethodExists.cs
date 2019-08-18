using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.BulkShippingUpdate.Rules
{
    //public class ShippingMethodExists : IBulkShippingUpdateValidationRule
    //{
    //    private readonly IShippingMethodAdminService _shippingMethodAdminService;

    //    public ShippingMethodExists(IShippingMethodAdminService shippingMethodAdminService)
    //    {
    //        _shippingMethodAdminService = shippingMethodAdminService;
    //    }

    //    public IEnumerable<string> GetErrors(BulkShippingUpdateDataTransferObject item)
    //    {
    //        var shippingMethod = _shippingMethodAdminService.GetAll().FirstOrDefault(x => x.DisplayName == item.ShippingMethod);
    //        if (shippingMethod == null)
    //            yield return string.Format("Shipping method {0} does not exist in the system.", item.ShippingMethod);
    //    }
    //}
}