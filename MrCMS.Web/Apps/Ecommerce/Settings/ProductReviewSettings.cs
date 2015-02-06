using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class ProductReviewSettings : SiteSettingsBase
    {
        public ProductReviewSettings()
        {
            PageSize = 10;
        }

        [DisplayName("Enable Product Reviews?")]
        public bool EnableProductReviews { get; set; }

        [DisplayName("Enable Guest Reviews?")]
        public bool GuestReviews { get; set; }

        [DisplayName("Enable Helpfulness Votes?")]
        public bool HelpfulnessVotes { get; set; }

        [DisplayName("Page Size")]
        public int PageSize { get; set; }
    }
}