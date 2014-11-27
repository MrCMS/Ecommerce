using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class ProductReviewSettings : SiteSettingsBase
    {
        [DisplayName("Product Reviews")]
        public bool EnableProductReviews { get; set; }

        [DisplayName("Guest Reviews")]
        public bool GuestReviews { get; set; }
        [DisplayName("Helpfulness Votes")]
        public bool HelpfulnessVotes { get; set; }

        [DisplayName("Page Size")]
        public int PageSize { get; set; }

        public ProductReviewSettings()
        {
            PageSize = 10;
        }
    }
}
