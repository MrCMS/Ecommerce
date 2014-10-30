namespace MrCMS.Web.Apps.Stats.Areas.Admin.Models
{
    public class PageViewResult
    {
        public string Url { get; set; }
        public int? WebpageId { get; set; }
        public string WebpageName { get; set; }
        public int Unique { get; set; }
        public int Sessions { get; set; }
        public int Total { get; set; }
    }
}