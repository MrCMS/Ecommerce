using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public interface IGetContentItemTemplateData
    {
        T Get<T>(NewsletterTemplate template) where T : ContentItemTemplateData;
    }
}