using System.IO;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceProducts;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public interface IAmazonApiService
    {
        bool IsLive(AmazonApiSection apiSection);

        MarketplaceWebServiceClient GetFeedsApiService();
        MarketplaceWebServiceProductsClient GetProductsApiService();

        GetMatchingProductForIdRequest GetMatchingProductForIdRequest(string sku);

        GetFeedSubmissionListRequest GetFeedSubmissionListRequest(string submissionId);
        SubmitFeedRequest GetSubmitFeedRequest(AmazonFeedType feedType, FileStream feedContent);
    }
}