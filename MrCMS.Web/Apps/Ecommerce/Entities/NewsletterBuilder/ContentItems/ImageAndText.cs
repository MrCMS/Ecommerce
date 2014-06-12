using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems
{
    public class ImageAndText : ContentItem
    {
        public virtual string Text { get; set; }
        [DisplayName("Image URL")]
        public virtual string ImageUrl { get; set; }
    }
}