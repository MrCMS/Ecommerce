using System;
using System.Linq;
using MarketplaceWebService;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Products
{
    public class AmazonProductsApiService : IAmazonProductsApiService
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonProductsApiService(
            AmazonSellerSettings amazonSellerSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService, IAmazonApiService amazonApiService)
        {
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _amazonApiService = amazonApiService;
        }

        public AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");

                var productsApiService = _amazonApiService.GetProductsApiService();
                var productsApiRequest = new GetServiceStatusRequest
                    {
                        SellerId = _amazonSellerSettings.SellerId
                    };
                var productsApiResult = productsApiService.GetServiceStatus(productsApiRequest);
                if (productsApiResult != null && productsApiResult.GetServiceStatusResult != null)
                    return productsApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();


            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return AmazonServiceStatus.RED;
        }

        public Product GetMatchingProductForId(string sku)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Products, 
                    null, null, "GetMatchingProductForId", "Get matching Amazon Product For SKU:" + sku);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = _amazonApiService.GetProductsApiService();
                var request = GetMatchingProductForIdRequest(sku);

                var result = service.GetMatchingProductForId(request);

                if (result != null && result.IsSetGetMatchingProductForIdResult()
                    && result.GetMatchingProductForIdResult != null && result.GetMatchingProductForIdResult.Any()
                    && result.GetMatchingProductForIdResult.First().status=="Success"
                    && result.GetMatchingProductForIdResult.First().Products.Product.Any())
                    return result.GetMatchingProductForIdResult.First().Products.Product.First();
            }
            catch (MarketplaceWebServiceException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error,
                    ex, null, AmazonApiSection.Feeds, "GetFeedSubmissionList", "Error happened during operation of getting matching Amazon Product For SKU:" + sku);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private GetMatchingProductForIdRequest GetMatchingProductForIdRequest(string sku)
        {
            return new GetMatchingProductForIdRequest()
            {
                IdType = "SellerSKU",
                IdList = new IdListType().WithId(sku),
                MarketplaceId = _amazonSellerSettings.MarketplaceId,
                SellerId = _amazonSellerSettings.SellerId
            };
        }
    }
}