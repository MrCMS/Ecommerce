using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingMethodController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingMethodAdminService _shippingMethodAdminService;

        public ShippingMethodController(IShippingMethodAdminService shippingMethodAdminService)
        {
            _shippingMethodAdminService = shippingMethodAdminService;
        }

        public ViewResult Index()
        {
            return View(_shippingMethodAdminService.GetAll());
        }
    }

    public interface IShippingMethodAdminService
    {
        List<ShippingMethodInfo> GetAll();
    }

    public class ShippingMethodAdminService : IShippingMethodAdminService
    {
        private readonly IEnumerable<IShippingMethod> _shippingMethods;
        private readonly UrlHelper _urlHelper;
        private readonly ShippingMethodSettings _shippingMethodSettings;

        public ShippingMethodAdminService(IEnumerable<IShippingMethod> shippingMethods, UrlHelper urlHelper, ShippingMethodSettings shippingMethodSettings)
        {
            _shippingMethods = shippingMethods;
            _urlHelper = urlHelper;
            _shippingMethodSettings = shippingMethodSettings;
        }

        public List<ShippingMethodInfo> GetAll()
        {
            var enabledMethods = _shippingMethodSettings.EnabledMethods;
            return _shippingMethods.Select(method =>
            {
                var typeName = method.GetType().FullName;
                return new ShippingMethodInfo
                {
                    Name = method.Name,
                    Description = method.Description,
                    Type = typeName,
                    ConfigureUrl = GetConfigureUrl(method),
                    Enabled = enabledMethods.ContainsKey(typeName) && enabledMethods[typeName]
                };
            }).OrderBy(info => info.Name).ToList();
        }

        private string GetConfigureUrl(IShippingMethod method)
        {
            if (!string.IsNullOrWhiteSpace(method.ConfigureAction) && !string.IsNullOrWhiteSpace(method.ConfigureController))
                return _urlHelper.Action(method.ConfigureAction, method.ConfigureController);
            return null;
        }
    }

    public class ShippingMethodInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string ConfigureUrl { get; set; }
        public bool Configurable { get { return !string.IsNullOrWhiteSpace(ConfigureUrl); } }

        public bool Enabled { get; set; }
        public string Type { get; set; }
    }
}