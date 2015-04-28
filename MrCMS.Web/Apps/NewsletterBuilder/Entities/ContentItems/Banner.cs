using System.ComponentModel;
using System.Web.UI.WebControls;

namespace MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems
{
    public class Banner : ContentItem
    {
        [DisplayName("Link URL")]
        public virtual string LinkUrl { get; set; }
        [DisplayName("Image URL")]
        public virtual string ImageUrl { get; set; }
    }
}