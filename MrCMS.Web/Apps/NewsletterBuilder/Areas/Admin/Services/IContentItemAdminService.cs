using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public interface IContentItemAdminService
    {
        T GetNew<T>(int newsletterId) where T : ContentItem, new();

        void Add<T>(T contentItem) where T :ContentItem;
        void Update<T>(T contentItem) where T :ContentItem;
        void Delete<T>(T contentItem) where T : ContentItem;
    }
}