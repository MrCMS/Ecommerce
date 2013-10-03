using System.Collections.Generic;
using System.IO;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonRequestService
    {
        List<string> SubmitMainFeeds(AmazonSyncModel model, List<FileStream> feeds);
        string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent);

        void CheckIfDeleteRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, string submissionId);
        void CheckIfRequestsWhereProcessed(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds);
        void CheckIfRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, List<string> submissionIds);

        string SubmitOrderFulfillmentFeed(AmazonSyncModel model, FileStream feedContent);
        void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, AmazonOrder amazonOrder,string submissionId);
        void CheckIfOrderFulfillmentFeedWasProcessed(AmazonSyncModel model, List<AmazonOrder> amazonOrders,string submissionId);
    }
}