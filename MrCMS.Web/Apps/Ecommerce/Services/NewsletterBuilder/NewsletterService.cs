using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public class NewsletterService : INewsletterService
    {
        private readonly ISession _session;

        public NewsletterService(ISession session)
        {
            _session = session;
        }

        public IList<Newsletter> GetAll()
        {
            return _session.QueryOver<Newsletter>().Cacheable().List();
        }

        public void Add(Newsletter newsletter)
        {
            _session.Transact(session => session.Save(newsletter));
        }

        public void Edit(Newsletter newsletter)
        {
            _session.Transact(session => session.Update(newsletter));
        }

        public void Delete(Newsletter newsletter)
        {
            _session.Transact(session => session.Delete(newsletter));
        }
    }
}