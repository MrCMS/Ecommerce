using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface INotificationTemplateSettingsManager
    {
        IList<NotificationTemplateSettings> GetAll();
        NotificationTemplateSettings Get(int id);
        void Save(NotificationTemplateSettings item);
        void Delete(NotificationTemplateSettings item);
    }
}