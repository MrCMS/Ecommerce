using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductReviewExtensions
    {
        public static int Upvotes(this ProductReview productReview)
        {
            return productReview == null
                ? 0
                : MrCMSApplication.Get<ISession>()
                    .QueryOver<HelpfulnessVote>()
                    .Where(vote => vote.ProductReview.Id == productReview.Id && vote.IsHelpful)
                    .Cacheable()
                    .RowCount();
        }
        public static int Downvotes(this ProductReview productReview)
        {
            return productReview == null
                ? 0
                : MrCMSApplication.Get<ISession>()
                    .QueryOver<HelpfulnessVote>()
                    .Where(vote => vote.ProductReview.Id == productReview.Id && !vote.IsHelpful)
                    .Cacheable()
                    .RowCount();
        }
    }
}