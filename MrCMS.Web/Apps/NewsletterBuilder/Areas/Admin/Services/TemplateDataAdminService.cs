using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using NHibernate;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public class TemplateDataAdminService : ITemplateDataAdminService
    {
        private readonly ISession _session;

        public TemplateDataAdminService(ISession session)
        {
            _session = session;
        }

        public T GetNew<T>(int templateId) where T : ContentItemTemplateData, new()
        {
            var newsletterTemplate = _session.Get<NewsletterTemplate>(templateId);
            if (newsletterTemplate == null)
                return null;
            return new T
            {
                NewsletterTemplate = newsletterTemplate
            };
        }

        public void Add<T>(T data) where T : ContentItemTemplateData
        {
            _session.Transact(session => session.Save(data));
        }

        public void Update<T>(T data) where T : ContentItemTemplateData
        {
            _session.Transact(session => session.Update(data));
        }
    }
}