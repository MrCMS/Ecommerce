using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class HelpfulnessVoteModel : IHaveIPAddress
    {
        public int ProductReviewId { get; set; }

        public string IPAddress { get; set; }
    }

    public class HelpfulnessVoteResponse : IProductReviewResponseInfo
    {
        public ProductReviewResponseType Type { get; set; }

        public string Message { get; set; }

        public string RedirectUrl { get; set; }
    }

    public interface IProductReviewResponseInfo
    {
        ProductReviewResponseType Type { get; }
        string Message { get; }
        string RedirectUrl { get; }
    }

    public enum ProductReviewResponseType
    {
        Success,
        Info,
        Error
    }

    public static class ProductReviewResponseInfoExtensions
    {
        public static bool IsSuccess(this IProductReviewResponseInfo producReviewResponseInfo)
        {
            return producReviewResponseInfo != null && producReviewResponseInfo.Type == ProductReviewResponseType.Success;
        }
    }

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