using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Search
{
    public class SearchLog : SiteEntity
    {
        public virtual string Text { get; set; }

        public virtual User User { get; set; }
    }
}