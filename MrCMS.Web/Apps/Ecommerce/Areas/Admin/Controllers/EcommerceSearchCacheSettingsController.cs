using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
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
        [MrCMSACLRule(typeof(EcommerceSearchCacheSettingsACL), EcommerceSearchCacheSettingsACL.Edit)]
        public ActionResult Edit()
        {
            return View(_ecommerceSearchCacheSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        [MrCMSACLRule(typeof(EcommerceSearchCacheSettingsACL), EcommerceSearchCacheSettingsACL.Edit)]
        public RedirectToRouteResult Edit_POST(EcommerceSearchCacheSettings ecommerceSearchCacheSettings)
        {
            _configurationProvider.SaveSettings(ecommerceSearchCacheSettings);
            return RedirectToAction("Edit");
        }
    }

    public class ProductReviewSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ProductReviewSettings _productReviewSettings;

        public ProductReviewSettingsController(IConfigurationProvider configurationProvider, ProductReviewSettings productReviewSettings)
        {
            _configurationProvider = configurationProvider;
            _productReviewSettings = productReviewSettings;
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return View(_productReviewSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(ProductReviewSettings productReviewSettings)
        {
            _configurationProvider.SaveSettings(productReviewSettings);
            return RedirectToAction("Edit");
        }
    }
}