namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class HelpfulnessVoteResponse : IProductReviewResponseInfo
    {
        public ProductReviewResponseType Type { get; set; }

        public string Message { get; set; }

        public string RedirectUrl { get; set; }
    }
}