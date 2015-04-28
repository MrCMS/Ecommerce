using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using NHibernate;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public class NewsletterTemplateAdminService : INewsletterTemplateAdminService
    {
        private readonly ISession _session;

        public NewsletterTemplateAdminService(ISession session)
        {
            _session = session;
        }

        public IList<NewsletterTemplate> GetAll()
        {
            return _session.QueryOver<NewsletterTemplate>().Cacheable().List();
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

        public HashSet<Type> GetTemplateDataOptions()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<ContentItemTemplateData>();
        }
    }
}