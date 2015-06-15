using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems
{
    public abstract class ContentItem : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }

        public virtual Newsletter Newsletter { get; set; }

        public virtual int DisplayOrder { get; set; }
    }
}