using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public class ContentItemService : IContentItemService
    {
        private readonly ISession _session;

        public ContentItemService(ISession session)
        {
            _session = session;
        }

        public void Add(ContentItem contentItem)
        {
            _session.Transact(session => session.Save(contentItem));
        }

        public void Edit(ContentItem contentItem)
        {
            _session.Transact(session => session.Update(contentItem));
        }

        public void Delete(ContentItem contentItem)
        {
            _session.Transact(session => session.Delete(contentItem));
        }
    }
}