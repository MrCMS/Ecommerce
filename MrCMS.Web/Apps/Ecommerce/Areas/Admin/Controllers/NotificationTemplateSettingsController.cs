using MrCMS.Web.Apps.Ecommerce.Binders;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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

        public ViewResult Index()
        {
            var notificationTemplateSettings = _notificationTemplateSettingsManager.GetAll();
            return View(notificationTemplateSettings);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new NotificationTemplateSettings() { Name = "Default" });
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToRouteResult Add_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);
            return RedirectToAction("Edit");
        }

        [HttpGet]
        public ActionResult Edit()
        {
            IList<NotificationTemplateSettings> notificationTemplateSettings = _notificationTemplateSettingsManager.GetAll();
            if (notificationTemplateSettings.Count()>0)
                return View(notificationTemplateSettings.First());
            else
                return RedirectToAction("Add");
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);

            return RedirectToAction("Edit");
        }

        [HttpGet]
        public PartialViewResult Delete(NotificationTemplateSettings notificationTemplateSettings)
        {
            return PartialView(notificationTemplateSettings);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Delete(notificationTemplateSettings);

            return RedirectToAction("Index");
        }
    }
}
