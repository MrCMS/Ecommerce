using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public abstract class Feedback : SiteEntity
    {
        public virtual FeedbackRecord FeedbackRecord { get; set; }
        public virtual string Message { get; set; }
        public virtual int Rating { get; set; }
    }

    public class FeedbackFacetRecord : Feedback
    {
        public virtual FeedbackFacet FeedbackFacet { get; set; }
    }

    public class ProductVariantFeedbackRecord : Feedback
    {
        public virtual ProductVariant ProductVariant { get; set; }
    }
}