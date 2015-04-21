using System;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Models
{
    public class PageViewSearchQuery
    {
        public PageViewSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Url { get; set; }
        [DisplayName("Search Type")]
        public PageViewSearchType SearchType { get; set; }
    }
}