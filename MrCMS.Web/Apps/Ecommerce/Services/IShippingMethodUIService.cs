using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IShippingMethodUIService
    {
        HashSet<IShippingMethod> GetEnabledMethods();
        IShippingMethod GetMethodByTypeName(string type);
    }
}