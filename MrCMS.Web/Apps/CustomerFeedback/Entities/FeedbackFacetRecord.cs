using MrCMS.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class FeedbackFacetRecord : SiteEntity
    {
        public virtual FeedbackRecord FeedbackRecord { get; set; }
        public virtual FeedbackFacet FeedbackFacet { get; set; }
        public virtual string Message { get; set; }
        public virtual int Rating { get; set; }
    }
}