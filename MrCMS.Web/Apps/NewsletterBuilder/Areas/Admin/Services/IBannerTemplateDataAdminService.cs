using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public interface ITemplateDataAdminService
    {
        T GetNew<T>(int templateId) where T : ContentItemTemplateData, new();
        void Add<T>(T data) where T : ContentItemTemplateData;
        void Update<T>(T data) where T : ContentItemTemplateData;
    }
}