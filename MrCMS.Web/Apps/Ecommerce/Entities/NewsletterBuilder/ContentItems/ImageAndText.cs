using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems
{
    public class ImageAndText : ContentItem
    {
        public virtual string Text { get; set; }
        public virtual string ImageUrl { get; set; }
    }
}