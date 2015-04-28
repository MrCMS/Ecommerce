using MrCMS.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData
{
    public abstract class ContentItemTemplateData : SiteEntity
    {
        public virtual NewsletterTemplate NewsletterTemplate { get; set; }
    }
}