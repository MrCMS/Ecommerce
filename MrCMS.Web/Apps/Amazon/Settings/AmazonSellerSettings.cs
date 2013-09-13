using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Amazon.Settings
{
    public class AmazonSellerSettings : SiteSettingsBase
    {
        [DisplayName("Seller / Merchant ID")]
        public string SellerId { get; set; }

        [DisplayName("Marketplace Id")]
        public string MarketplaceId { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}