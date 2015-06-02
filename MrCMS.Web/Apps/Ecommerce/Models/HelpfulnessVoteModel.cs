using MrCMS.Web.Apps.Ecommerce.ModelBinders;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class HelpfulnessVoteModel : IHaveIPAddress
    {
        public int ProductReviewId { get; set; }

        public string IPAddress { get; set; }
    }
}