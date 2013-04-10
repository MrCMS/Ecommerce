using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class NotificationTemplateSettingsManager : INotificationTemplateSettingsManager
    {
        private readonly ISession _session;

        public NotificationTemplateSettingsManager(ISession session)
        {
            _session = session;
        }

        public IList<NotificationTemplateSettings> GetAll()
        {
            return _session.QueryOver<NotificationTemplateSettings>().Cacheable().List();
        }

        public NotificationTemplateSettings Get(int id)
        {
            return _session.QueryOver<NotificationTemplateSettings>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public void Save(NotificationTemplateSettings item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(NotificationTemplateSettings item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}