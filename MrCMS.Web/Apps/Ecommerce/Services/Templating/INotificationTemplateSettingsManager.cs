using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Templating;

namespace MrCMS.Web.Apps.Ecommerce.Services.Templating
{
    public interface INotificationTemplateSettingsManager
    {
        NotificationTemplateSettings Get();
        //NotificationTemplateSettings Get(int id);
        void Save(NotificationTemplateSettings item);
        void Delete(NotificationTemplateSettings item);
    }
}