using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder
{
    public class Newsletter : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual NewsletterTemplate NewsletterTemplate { get; set; }
        public virtual IList<ContentItem> ContentItems { get; set; }
    }
}