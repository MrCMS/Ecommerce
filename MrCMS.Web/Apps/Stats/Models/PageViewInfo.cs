using System;

namespace MrCMS.Web.Apps.Stats.Models
{
    public class PageViewInfo
    {
        public Guid User { get; set; }
        public Guid Session { get; set; }
        public string Url { get; set; }
    }
}