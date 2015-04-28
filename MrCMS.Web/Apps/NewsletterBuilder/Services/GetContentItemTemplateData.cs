using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using NHibernate;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public class GetContentItemTemplateData : IGetContentItemTemplateData
    {
        private readonly ISession _session;

        public GetContentItemTemplateData(ISession session)
        {
            _session = session;
        }

        public T Get<T>(NewsletterTemplate template) where T : ContentItemTemplateData
        {
            return template == null
                ? null
                : _session.QueryOver<T>()
                    .Where(arg => arg.NewsletterTemplate.Id == template.Id)
                    .Cacheable()
                    .Take(1)
                    .SingleOrDefault();
        }
    }
}