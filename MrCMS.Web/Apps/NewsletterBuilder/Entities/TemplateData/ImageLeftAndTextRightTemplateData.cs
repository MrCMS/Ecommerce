using System.ComponentModel;
using MrCMS.Web.Apps.NewsletterBuilder.Attributes;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData
{
    [TemplateDataFor(typeof (ImageLeftAndTextRight))]
    public class ImageLeftAndTextRightTemplateData : ContentItemTemplateData
    {
        [DisplayName("Image Left and Text Right Template")]
        public virtual string ImageLeftAndTextRightTemplate { get; set; }
    }
}