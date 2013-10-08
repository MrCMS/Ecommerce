using MarketplaceWebService;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceProducts;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public class AmazonApiService : IAmazonApiService
    {
        private readonly AmazonAppSettings _amazonAppSettings;

        public AmazonApiService(AmazonAppSettings amazonAppSettings)
        {
            _amazonAppSettings = amazonAppSettings;
        }

        public MarketplaceWebServiceClient GetFeedsApiService()
        {
            var config = new MarketplaceWebServiceConfig() { ServiceURL = _amazonAppSettings.ApiEndpoint };
            return new MarketplaceWebServiceClient(MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey, "MrCMS", MrCMSApplication.AssemblyVersion,
                                                    config);
        }
        public MarketplaceWebServiceProductsClient GetProductsApiService()
        {
            var config = new MarketplaceWebServiceProductsConfig() { ServiceURL = _amazonAppSettings.ProductsApiEndpoint };
            return new MarketplaceWebServiceProductsClient("MrCMS", MrCMSApplication.AssemblyVersion,MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey,
                                                    config);
        }
        public MarketplaceWebServiceOrdersClient GetOrdersApiService()
        {
            var config = new MarketplaceWebServiceOrdersConfig() { ServiceURL = _amazonAppSettings.OrdersApiEndpoint };
            return new MarketplaceWebServiceOrdersClient("MrCMS", MrCMSApplication.AssemblyVersion, _amazonAppSettings.AWSAccessKeyId,
                                                         _amazonAppSettings.SecretKey, config);
        }
    }
}