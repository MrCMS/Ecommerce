using System.ComponentModel;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Amazon.Settings
{
    public class AmazonSellerSettings : SiteSettingsBase
    {
        [DisplayName("Seller / Merchant ID")]
        public string SellerId { get; set; }

        [DisplayName("Marketplace Id")]
        public string MarketplaceId { get; set; }

        [DisplayName("When creating Amazon listing's Standard Product Id, use Product Variant Barcode value as:")]
        public StandardProductIDType BarcodeIsOfType { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}