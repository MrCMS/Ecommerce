using System;
using System.ComponentModel;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Models
{
    public class PageViewSearchQuery
    {
        public PageViewSearchQuery()
        {
            Page = 1;
            From = CurrentRequestData.Now.AddMonths(-1);
            To = CurrentRequestData.Now;
        }

        public int Page { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Url { get; set; }
        [DisplayName("Search Type")]
        public PageViewSearchType SearchType { get; set; }
    }
}