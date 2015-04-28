using System.ComponentModel;
using MrCMS.Web.Apps.NewsletterBuilder.Attributes;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData
{
    [TemplateDataFor(typeof(FreeText))]
    public class FreeTextTemplateData : ContentItemTemplateData
    {
        [DisplayName("Free Text Template")]
        public virtual string FreeTextTemplate { get; set; }
    }
}