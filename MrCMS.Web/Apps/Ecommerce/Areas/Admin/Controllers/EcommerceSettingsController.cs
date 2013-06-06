using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly EcommerceSettings _ecommerceSettings;

        public EcommerceSettingsController(IConfigurationProvider configurationProvider, EcommerceSettings ecommerceSettings)
        {
            _configurationProvider = configurationProvider;
            _ecommerceSettings = ecommerceSettings;
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (_ecommerceSettings.CategoryProductsPerPage == String.Empty)
                _ecommerceSettings.CategoryProductsPerPage = "12,20,40";
            if (_ecommerceSettings.PageSizeAdmin == 0)
                _ecommerceSettings.PageSizeAdmin = 20;
            return View(_ecommerceSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(EcommerceSettings ecommerceSettings)
        {
            _configurationProvider.SaveSettings(ecommerceSettings);
            return RedirectToAction("Edit");
        }
    }
}
