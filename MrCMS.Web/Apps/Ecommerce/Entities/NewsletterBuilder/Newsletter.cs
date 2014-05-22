using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder
{
    public class Newsletter : SiteEntity
    {
        public virtual string Name { get; set; }
    }
}