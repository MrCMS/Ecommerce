using MrCMS.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class FeedbackFacet : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}