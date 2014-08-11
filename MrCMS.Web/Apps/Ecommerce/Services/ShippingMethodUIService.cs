using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ShippingMethodUIService : IShippingMethodUIService
    {
        private readonly HashSet<IShippingMethod> _enabledShippingMethods;
        private readonly IKernel _kernel;

        public ShippingMethodUIService(IKernel kernel, ShippingMethodSettings shippingMethodSettings)
        {
            _kernel = kernel;
            _enabledShippingMethods =
                shippingMethodSettings.GetEnabledMethods()
                    .Select(type => _kernel.Get(type) as IShippingMethod)
                    .ToHashSet();
        }

        public HashSet<IShippingMethod> GetEnabledMethods()
        {
            return _enabledShippingMethods;
        }

        public IShippingMethod GetMethodByTypeName(string type)
        {
            return _enabledShippingMethods.FirstOrDefault(method => method.GetType().FullName == type);
        }
    }
}