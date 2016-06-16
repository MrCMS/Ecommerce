namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class FeedbackFacetRecord : Feedback
    {
        public virtual FeedbackFacet FeedbackFacet { get; set; }
    }
}