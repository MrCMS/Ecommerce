using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews
{
    public class HelpfulnessVote : SiteEntity
    {
        public virtual Review Review { get; set; }

        public virtual User User { get; set; }

        public virtual bool IsHelpful { get; set; }
    }
}