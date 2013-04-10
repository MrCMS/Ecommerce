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
        private readonly INotificationTemplateProcessor _notificationTemplateProcessor;

        public NotificationTemplateSettingsController(INotificationTemplateSettingsManager notificationTemplateSettingsManager, INotificationTemplateProcessor notificationTemplateProcessor)
        {
            _notificationTemplateSettingsManager = notificationTemplateSettingsManager;
            _notificationTemplateProcessor = notificationTemplateProcessor;
        }

        public string Test()
        {
            Region provider = new Region();
            provider.Name = "Will Anderson";

            NotificationTemplateSettings notificationTemplateSettings = new NotificationTemplateSettings();
            notificationTemplateSettings.OrderConfirmationTemplate = "Hi {Name}! Your order is being processed. Total amount of order is: ${GetOrderTotalAmount()}";

            return _notificationTemplateProcessor.ReplaceTokensAndMethods<Region>(provider, notificationTemplateSettings.OrderConfirmationTemplate);
        }

        public ViewResult Index()
        {
            var notificationTemplateSettings = _notificationTemplateSettingsManager.GetAll();
            return View(notificationTemplateSettings);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new NotificationTemplateSettings());
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToRouteResult Add_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Edit(NotificationTemplateSettings notificationTemplateSettings)
        {
            return View(notificationTemplateSettings);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(NotificationTemplateSettings notificationTemplateSettings)
        {
            _notificationTemplateSettingsManager.Save(notificationTemplateSettings);

            return RedirectToAction("Index");
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
