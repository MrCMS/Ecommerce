using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using NHibernate;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public class ContentItemAdminService : IContentItemAdminService
    {
        private readonly ISession _session;

        public ContentItemAdminService(ISession session)
        {
            _session = session;
        }

        public T GetNew<T>(int newsletterId) where T : ContentItem, new()
        {
            var newsletter = _session.Get<Newsletter>(newsletterId);
            if (newsletter == null)
                return null;
            return new T
            {
                Newsletter = newsletter
            };
        }

        public void Add<T>(T contentItem) where T : ContentItem
        {
            _session.Transact(session =>
            {
                contentItem.Newsletter.ContentItems.Add(contentItem);
                session.Save(contentItem);
                session.Update(contentItem.Newsletter);
            });
        }

        public void Update<T>(T contentItem) where T : ContentItem
        {
            _session.Transact(session => session.Update(contentItem));
        }

        public void Delete<T>(T contentItem) where T : ContentItem
        {
            _session.Transact(session =>
            {
                contentItem.Newsletter.ContentItems.Remove(contentItem);
                session.Delete(contentItem);
                session.Update(contentItem.Newsletter);
            });
        }
    }
}