using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
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