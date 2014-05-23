using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public interface IContentItemService
    {
        void Add(ContentItem contentItem);
        void Edit(ContentItem contentItem);
        void Delete(ContentItem contentItem);
    }
}