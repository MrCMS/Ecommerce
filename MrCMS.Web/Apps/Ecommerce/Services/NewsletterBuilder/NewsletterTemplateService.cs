using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public class NewsletterTemplateService : INewsletterTemplateService
    {
        private readonly ISession _session;

        public NewsletterTemplateService(ISession session)
        {
            _session = session;
        }

        public IList<NewsletterTemplate> GetAll()
        {
            return _session.QueryOver<NewsletterTemplate>().Cacheable().List();
        }

        public IPagedList<NewsletterTemplate> GetAllPaged()
        {
            return null;
            //return _session.QueryOver<NewsletterTemplate>().Paged(,10);
        }

        public void Add(NewsletterTemplate newsletterTemplate)
        {
            _session.Transact(session => session.Save(newsletterTemplate));
        }

        public void Edit(NewsletterTemplate newsletterTemplate)
        {
            _session.Transact(session => session.Update(newsletterTemplate));
        }

        public void Delete(NewsletterTemplate newsletterTemplate)
        {
            _session.Transact(session => session.Delete(newsletterTemplate));
        }
    }
}