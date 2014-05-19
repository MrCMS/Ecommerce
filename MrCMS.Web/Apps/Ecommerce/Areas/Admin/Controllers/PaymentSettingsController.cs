using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PaymentSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly PaymentSettings _paymentSettings;

        public PaymentSettingsController(IConfigurationProvider configurationProvider, PaymentSettings paymentSettings)
        {
            _configurationProvider = configurationProvider;
            _paymentSettings = paymentSettings;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_paymentSettings);
        }

        [HttpPost]
        public ActionResult Save([IoCModelBinder(typeof(PaymentSettingsModelBinder))] PaymentSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}