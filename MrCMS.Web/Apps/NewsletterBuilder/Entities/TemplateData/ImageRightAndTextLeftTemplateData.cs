using System.ComponentModel;
using MrCMS.Web.Apps.NewsletterBuilder.Attributes;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData
{
    [TemplateDataFor(typeof(ImageRightAndTextLeft))]
    public class ImageRightAndTextLeftTemplateData : ContentItemTemplateData
    {
        [DisplayName("Image Right and Text Left Template")]
        public virtual string ImageRightAndTextLeftTemplate { get; set; }
    }
}