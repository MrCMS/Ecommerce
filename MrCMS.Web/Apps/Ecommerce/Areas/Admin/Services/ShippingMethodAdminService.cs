using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ShippingMethodAdminService : IShippingMethodAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IEnumerable<IShippingMethod> _shippingMethods;
        private readonly UrlHelper _urlHelper;

        public ShippingMethodAdminService(IEnumerable<IShippingMethod> shippingMethods, UrlHelper urlHelper,
            IConfigurationProvider configurationProvider)
        {
            _shippingMethods = shippingMethods;
            _urlHelper = urlHelper;
            _configurationProvider = configurationProvider;
        }

        public List<ShippingMethodInfo> GetAll()
        {
            var shippingMethodSettings = _configurationProvider.GetSiteSettings<ShippingMethodSettings>();
            Dictionary<string, bool> enabledMethods = shippingMethodSettings.EnabledMethods;
            return _shippingMethods.Select(method =>
            {
                string typeName = method.GetType().FullName;
                return new ShippingMethodInfo
                {
                    Name = method.Name,
                    DisplayName = method.DisplayName,
                    Description = method.Description,
                    Type = typeName,
                    ConfigureUrl = GetConfigureUrl(method),
                    Enabled = enabledMethods.ContainsKey(typeName) && enabledMethods[typeName]
                };
            }).OrderBy(info => info.Name).ToList();
        }

        public void UpdateSettings(ShippingMethodSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }

        private string GetConfigureUrl(IShippingMethod method)
        {
            if (!string.IsNullOrWhiteSpace(method.ConfigureAction) &&
                !string.IsNullOrWhiteSpace(method.ConfigureController))
                return _urlHelper.Action(method.ConfigureAction, method.ConfigureController);
            return null;
        }
    }
}