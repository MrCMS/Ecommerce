using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingCountryIsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingCountryIsAdminService _shippingCountryIsAdminService;

        public ShippingCountryIsController(IShippingCountryIsAdminService shippingCountryIsAdminService)
        {
            _shippingCountryIsAdminService = shippingCountryIsAdminService;
        }

        public PartialViewResult Fields(ShippingCountryIs shippingCountryIs)
        {
            ViewData["shipping-country-options"] = _shippingCountryIsAdminService.GetCountryOptions();
            return PartialView(shippingCountryIs);
        }
    }
}