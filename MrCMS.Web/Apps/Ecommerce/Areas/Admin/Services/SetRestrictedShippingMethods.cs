using System;
using System.Collections.Specialized;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class SetRestrictedShippingMethods : ISetRestrictedShippingMethods
    {
        private const string ShippingMethodPrefix = "restricted-shipping-";
        public void SetMethods(ProductVariant productVariant, NameValueCollection requestData)
        {
            productVariant.RestrictedTo.Clear();
            if (!productVariant.HasRestrictedShipping)
            {
                return;
            }
            foreach (var key in requestData.AllKeys.Where(s => s.StartsWith(ShippingMethodPrefix))
                .Where(key => requestData[key].Contains("true", StringComparison.OrdinalIgnoreCase)))
            {
                productVariant.RestrictedTo.Add(key.Replace(ShippingMethodPrefix, string.Empty));
            }
        }
    }
}