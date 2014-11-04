using MarketplaceWebServiceFeedsClasses;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Core.Services.Installation;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon
{
    public class AmazonAppInstallation : IOnInstallation
    {
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IConfigurationProvider _configurationProvider;

        public AmazonAppInstallation(AmazonAppSettings amazonAppSettings, AmazonSellerSettings amazonSellerSettings, IConfigurationProvider configurationProvider)
        {
            _amazonAppSettings = amazonAppSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _configurationProvider = configurationProvider;
        }


        public int Priority
        {
            get { return -1; }
        }

        public void Install(InstallModel model)
        {
            _amazonAppSettings.FeedsApiVersion = "2009-01-01";
            _amazonAppSettings.OrdersApiVersion = "2011-01-01";
            _amazonAppSettings.ProductsApiVersion = "2011-10-01";
            _amazonAppSettings.ApiEndpoint = "https://mws.amazonservices.co.uk/";
            _amazonAppSettings.AmazonProductDetailsUrl = "http://www.amazon.co.uk/gp/product/";
            _amazonAppSettings.AmazonOrderDetailsUrl = "https://sellercentral.amazon.co.uk/gp/orders-v2/details/?orderID=";
            _amazonAppSettings.AmazonManageOrdersUrl = "https://sellercentral.amazon.co.uk/gp/orders-v2/list";
            _amazonAppSettings.AmazonManageInventoryUrl = "https://sellercentral.amazon.co.uk/myi/search/ProductSummary";

            _configurationProvider.SaveSettings(_amazonAppSettings);

            _amazonSellerSettings.BarcodeIsOfType = StandardProductIDType.EAN;
            _configurationProvider.SaveSettings(_amazonSellerSettings);

            var folderLocation = string.Format("{0}/{1}/", CurrentRequestData.CurrentSite.Id, "amazon");
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory(folderLocation);
        }
    }
}