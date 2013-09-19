using System.Collections.Generic;
using System.IO;
using MarketplaceWebService.Model;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public interface IAmazonApiService
    {
        ServiceStatusEnum GetServiceStatus(AmazonApiSection apiSection);

        FeedSubmissionInfo GetFeedSubmissionList(string submissionId);
        FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent);
        FileStream GetProductFeedContent(AmazonListing listing);
        FileStream GetProductPriceFeedContent(AmazonListing listing);
        FileStream GetProductInventoryFeedContent(AmazonListing listing);
        FileStream GetProductImageFeedContent(AmazonListing listing);

        IEnumerable<Order> ListOrders(AmazonSyncModel model);
        IEnumerable<Order> GetOrder(AmazonSyncModel model);
        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
    }
}