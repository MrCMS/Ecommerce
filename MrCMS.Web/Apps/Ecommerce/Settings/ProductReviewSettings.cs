using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class ProductReviewSettings : SiteSettingsBase
    {
        [DisplayName("Product Reviews")]
        public virtual bool EnableProductReviews { get; set; }

        [DisplayName("Guest Reviews")]
        public virtual bool GuestReviews { get; set; }
        [DisplayName("Helpfulness Votes")]
        public virtual bool HelpfulnessVotes { get; set; }

        [DisplayName("Page Size")]
        public virtual int PageSize { get; set; }

        public ProductReviewSettings()
        {
            PageSize = 10;
        }
    }
}
