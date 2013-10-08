using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class SyncAmazonListingService : ISyncAmazonListingService
    {
        private readonly IAmazonListingRequestService _amazonRequestService;
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;

        public SyncAmazonListingService(IAmazonListingRequestService amazonRequestService, IAmazonFeedsApiService amazonFeedsApiService)
        {
            _amazonRequestService = amazonRequestService;
            _amazonFeedsApiService = amazonFeedsApiService;
        }

        public void SubmitProductFeeds(AmazonSyncModel model, AmazonListingGroup item)
        {
            var feeds = _amazonFeedsApiService.GetProductsMainFeeds(item);

            var submissionIds = _amazonRequestService.SubmitMainFeeds(model, feeds);

            _amazonRequestService.CheckIfRequestsWereProcessed(model, item, submissionIds);
        }
        
        public void SubmitSingleProductFeed(AmazonSyncModel model, AmazonListing item)
        {
            var feeds = _amazonFeedsApiService.GetSingleProductMainFeeds(item);

            var submissionIds = _amazonRequestService.SubmitMainFeeds(model, feeds);

            _amazonRequestService.CheckIfRequestWasProcessed(model, item, submissionIds);
        }
    }
}