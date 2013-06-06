using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using MrCMS.Web.Apps.Ecommerce.Services.Templating;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class NotificationTemplateSettingsController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly INotificationTemplateSettingsManager _notificationTemplateSettingsManager;

        public NotificationTemplateSettingsController(INotificationTemplateSettingsManager notificationTemplateSettingsManager)
        {
            _notificationTemplateSettingsManager = notificationTemplateSettingsManager;
        }

        [HttpGet]
        public ViewResult Edit()
        {
            NotificationTemplateSettings settings = _notificationTemplateSettingsManager.Get();
            if (settings == null)
                settings = new NotificationTemplateSettings();
            return View(settings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);
            return RedirectToAction("Edit");
        }

    }
}
