using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.MessageTemplates
{
    public class MessageTemplateSettingsManager : IMessageTemplateSettingsManager
    {
        private readonly ISession _session;

        public MessageTemplateSettingsManager(ISession session)
        {
            _session = session;
        }

        public IList<MessageTemplateSettings> GetAll()
        {
            return _session.QueryOver<MessageTemplateSettings>().Cacheable().List();
        }

        public MessageTemplateSettings Get(int id)
        {
            return _session.QueryOver<MessageTemplateSettings>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public MessageTemplateSettings Get()
        {
            return _session.QueryOver<MessageTemplateSettings>().Cacheable().SingleOrDefault();
        }

        public void Save(MessageTemplateSettings item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(MessageTemplateSettings item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}