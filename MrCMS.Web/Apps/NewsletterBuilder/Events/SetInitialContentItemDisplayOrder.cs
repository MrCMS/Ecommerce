using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.NewsletterBuilder.Events
{
    public class SetInitialContentItemDisplayOrder : IOnAdding<ContentItem>
    {
        public void Execute(OnAddingArgs<ContentItem> args)
        {
            ContentItem contentItem = args.Item;
            Newsletter newsletter = contentItem.Newsletter;
            if (newsletter == null)
                return;
            var session = args.Session;
            if (!GetBaseQuery(session, newsletter).Any())
                return;
            contentItem.DisplayOrder =
                GetBaseQuery(session, newsletter)
                    .Select(Projections.Max<ContentItem>(x => x.DisplayOrder))
                    .SingleOrDefault<int>() + 1;
        }

        private static IQueryOver<ContentItem, ContentItem> GetBaseQuery(ISession session,
            Newsletter newsletter)
        {
            return session.QueryOver<ContentItem>().Where(x => x.Newsletter.Id == newsletter.Id);
        }
    }
}