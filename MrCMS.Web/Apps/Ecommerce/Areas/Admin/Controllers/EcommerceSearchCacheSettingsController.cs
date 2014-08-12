using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSearchCacheSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly EcommerceSearchCacheSettings _ecommerceSearchCacheSettings;

        public EcommerceSearchCacheSettingsController(IConfigurationProvider configurationProvider, EcommerceSearchCacheSettings ecommerceSearchCacheSettings)
        {
            _configurationProvider = configurationProvider;
            _ecommerceSearchCacheSettings = ecommerceSearchCacheSettings;
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return View(_ecommerceSearchCacheSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(EcommerceSearchCacheSettings ecommerceSearchCacheSettings)
        {
            _configurationProvider.SaveSettings(ecommerceSearchCacheSettings);
            return RedirectToAction("Edit");
        }
    }
}