using System;
using System.Linq;
using MarketplaceWebService;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Products
{
    public class AmazonProductsApiService : IAmazonProductsApiService
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonProductsApiService(IAmazonAnalyticsService amazonAnalyticsService, 
            IAmazonLogService amazonLogService, IAmazonApiService amazonApiService)
        {
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _amazonApiService = amazonApiService;
        }

        public Product GetMatchingProductForId(string sku)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null,null,AmazonApiSection.Products, 
                    "GetMatchingProductForId", null,null,null,"Getting Amazon Product details with SKU:" + sku);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = _amazonApiService.GetProductsApiService();
                var request = _amazonApiService.GetMatchingProductForIdRequest(sku);

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
                    ex, null, AmazonApiSection.Feeds, "GetFeedSubmissionList", null,null,null,
                    "Error happened during operation of getting Amazon Product details with SKU:" + sku);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        
    }
}