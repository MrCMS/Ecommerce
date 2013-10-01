using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Amazon.Settings
{
    public class AmazonAppSettings : SiteSettingsBase
    {
        [DisplayName("Api Endpoint")]
        public string ApiEndpoint { get; set; }

        [DisplayName("Feeds Api Version")]
        public string FeedsApiVersion { get; set; }

        [DisplayName("Products Api Version")]
        public string ProductsApiVersion { get; set; }

        [DisplayName("Orders Api Version")]
        public string OrdersApiVersion { get; set; }

        [DisplayName("AWS Access Key ID")]
        public string AWSAccessKeyId { get; set; }

        [DisplayName("Secret Key")]
        public string SecretKey { get; set; }

        [DisplayName("Amazon Product Details Url")]
        public string AmazonProductDetailsUrl { get; set; }

        [DisplayName("Amazon Order Details Url")]
        public string AmazonOrderDetailsUrl { get; set; }

        [DisplayName("Amazon Manage Orders Url")]
        public string AmazonManageOrdersUrl { get; set; }

        [DisplayName("Amazon Manager Inventory Url")]
        public string AmazonManageInventoryUrl { get; set; }

        public string FeedsApiEndpoint
        {
            get { return ApiEndpoint + "doc/" + FeedsApiVersion; }
        }

        public string ProductsApiEndpoint
        {
            get { return ApiEndpoint + "Products/" + ProductsApiVersion; }
        }

        public string OrdersApiEndpoint
        {
            get { return ApiEndpoint + "Orders/" + OrdersApiVersion; }
        }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}