namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models
{
    public class FeedbackRecordAdminQuery
    {
        public FeedbackRecordAdminQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
    }
}