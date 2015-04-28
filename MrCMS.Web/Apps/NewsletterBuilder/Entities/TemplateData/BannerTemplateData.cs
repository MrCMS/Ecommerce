using System.ComponentModel;
using MrCMS.Web.Apps.NewsletterBuilder.Attributes;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData
{
    [TemplateDataFor(typeof(Banner))]
    public class BannerTemplateData : ContentItemTemplateData
    {
        [DisplayName("Banner Template")]
        public virtual string BannerTemplate { get; set; }
    }
}