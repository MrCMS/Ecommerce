using System.ComponentModel;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems
{
    public class ImageLeftAndTextRight : ContentItem
    {
        public virtual string Text { get; set; }
        [DisplayName("Image URL")]
        public virtual string ImageUrl { get; set; }
    }
}