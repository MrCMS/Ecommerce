using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems
{
    public class FreeText : ContentItem
    {
        public virtual string Text { get; set; }
    }
}