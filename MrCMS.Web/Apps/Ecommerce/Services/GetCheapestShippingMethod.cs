using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetCheapestShippingMethod : IGetCheapestShippingMethod
    {
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public GetCheapestShippingMethod(IShippingMethodUIService shippingMethodUIService)
        {
            _shippingMethodUIService = shippingMethodUIService;
        }

        public ShippingAmount Get(CartModel cart)
        {
            HashSet<IShippingMethod> availableMethods =
                _shippingMethodUIService.GetEnabledMethods()
                    .FindAll(method => method.GetShippingTotal(cart) != ShippingAmount.NoneAvailable);
            if (!availableMethods.Any())
                return ShippingAmount.NoneAvailable;
            return availableMethods.Select(method => method.GetShippingTotal(cart))
                .OrderBy(amount => amount.Value)
                .First();
        }
    }
}