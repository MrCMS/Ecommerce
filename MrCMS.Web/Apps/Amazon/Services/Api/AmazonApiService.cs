using System;
using System.Collections.Generic;
using System.IO;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceProducts;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using GetServiceStatusRequest = MarketplaceWebServiceOrders.Model.GetServiceStatusRequest;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public class AmazonApiService : IAmazonApiService
    {
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;

        public AmazonApiService(AmazonAppSettings amazonAppSettings, IAmazonLogService amazonLogService, IAmazonAnalyticsService amazonAnalyticsService, AmazonSellerSettings amazonSellerSettings)
        {
            _amazonAppSettings = amazonAppSettings;
            _amazonLogService = amazonLogService;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonSellerSettings = amazonSellerSettings;
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
        private MarketplaceWebServiceOrdersClient GetOrdersApiService()
        {
            var config = new MarketplaceWebServiceOrdersConfig() { ServiceURL = _amazonAppSettings.ProductsApiEndpoint };
            return new MarketplaceWebServiceOrdersClient("MrCMS", MrCMSApplication.AssemblyVersion, MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey,
                                                    config);
        }

        private AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, apiSection,
                                      "GetServiceStatus", null, null, null, "Getting Api Service Status");
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");
                switch (apiSection)
                {
                    case AmazonApiSection.Orders:
                        var ordersApiRequest = new GetServiceStatusRequest {SellerId = _amazonSellerSettings.SellerId};
                        var ordersApiResult = GetOrdersApiService().GetServiceStatus(ordersApiRequest);
                        if (ordersApiResult != null && ordersApiResult.GetServiceStatusResult != null)
                            return ordersApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();
                        break;
                    case AmazonApiSection.Products:
                        var productsApiRequest = new MarketplaceWebServiceProducts.Model.GetServiceStatusRequest
                            {
                                SellerId = _amazonSellerSettings.SellerId
                            };
                        var productsApiResult = GetProductsApiService().GetServiceStatus(productsApiRequest);
                        if (productsApiResult != null && productsApiResult.GetServiceStatusResult != null)
                            return productsApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();
                        break;
                }
            }
            catch (MarketplaceWebServiceProductsException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus",
                                      null, null, null);
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus",
                                      null, null, null);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return AmazonServiceStatus.RED;
        }
        public bool IsLive(AmazonApiSection apiSection)
        {
            var serviceStatus = GetServiceStatus(apiSection);
            return serviceStatus == AmazonServiceStatus.GREEN || serviceStatus == AmazonServiceStatus.GREEN_I;
        }

        public GetMatchingProductForIdRequest GetMatchingProductForIdRequest(string sku)
        {
            return new GetMatchingProductForIdRequest()
            {
                IdType = "SellerSKU",
                IdList = new IdListType().WithId(sku),
                MarketplaceId = _amazonSellerSettings.MarketplaceId,
                SellerId = _amazonSellerSettings.SellerId
            };
        }
        public GetFeedSubmissionListRequest GetFeedSubmissionListRequest(string submissionId)
        {
            return new GetFeedSubmissionListRequest()
            {
                Merchant = _amazonSellerSettings.SellerId,
                FeedSubmissionIdList = new IdList() { Id = new List<string>() { submissionId } }
            };
        }
        public SubmitFeedRequest GetSubmitFeedRequest(AmazonFeedType feedType, FileStream feedContent)
        {
            var request = new SubmitFeedRequest()
            {
                Merchant = _amazonSellerSettings.SellerId,
                ContentType = new ContentType(MediaType.XML),
                FeedContent = feedContent,
                FeedType = feedType.ToString(),
                MarketplaceIdList = new IdList { Id = new List<string>(new[] { _amazonSellerSettings.MarketplaceId }) }
            };
            request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
            return request;
        }
    }
}