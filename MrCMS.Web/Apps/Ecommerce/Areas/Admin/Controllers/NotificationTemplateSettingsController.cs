using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using MrCMS.Web.Apps.Ecommerce.Services.MessageTemplates;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class NotificationTemplateSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IMessageTemplateSettingsManager _notificationTemplateSettingsManager;

        public NotificationTemplateSettingsController(IMessageTemplateSettingsManager notificationTemplateSettingsManager)
        {
            _notificationTemplateSettingsManager = notificationTemplateSettingsManager;
        }

        [HttpGet]
        public ViewResult Edit()
        {
            var settings = _notificationTemplateSettingsManager.Get();
            if (settings == null)
                settings = new MessageTemplateSettings();
            return View(settings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(MessageTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);
            return RedirectToAction("Edit");
        }

    }
}
