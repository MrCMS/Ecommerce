using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.ACLRules;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.CustomerFeedback.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Controllers
{
    public class CustomerFeedbackSettingsController : MrCMSAppAdminController<CustomerFeedbackApp>
    {
        private readonly IConfigurationProvider _configurationProvider;

        public CustomerFeedbackSettingsController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [MrCMSACLRule(typeof(CustomerFeedbackSettingsACL), CustomerFeedbackSettingsACL.View)]
        public ViewResult Index()
        {
            return View(_configurationProvider.GetSiteSettings<CustomerFeedbackSettings>());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(CustomerFeedbackSettingsACL), CustomerFeedbackSettingsACL.View)]
        public RedirectToRouteResult Save([IoCModelBinder(typeof(CustomerFeedbackSettingsModelBinder))] CustomerFeedbackSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Customer Feedback Settings Saved");
            return RedirectToAction("Index");
        }
    }
}