using System.Linq;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ShippingMethodSubmenuGenerator : IShippingMethodSubmenuGenerator
    {
        private readonly IShippingMethodAdminService _shippingMethodAdminService;

        public ShippingMethodSubmenuGenerator(IShippingMethodAdminService shippingMethodAdminService)
        {
            _shippingMethodAdminService = shippingMethodAdminService;
        }

        public SubMenu Get()
        {
            var shippingMethodInfos = _shippingMethodAdminService.GetAll().FindAll(info => info.Enabled);
            var subMenu = new SubMenu();
            var items = shippingMethodInfos.Select(shippingMethodInfo => new ChildMenuItem(shippingMethodInfo.Name, shippingMethodInfo.ConfigureUrl)).ToList();
            items.Insert(0, new ChildMenuItem("Configuration", "/Admin/Apps/Ecommerce/ShippingMethod"));
            subMenu.Add("", items);
            return subMenu;
        }
    }
}