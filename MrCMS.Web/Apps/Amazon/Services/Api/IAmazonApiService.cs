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
        AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection);

        FeedSubmissionInfo GetFeedSubmissionList(string submissionId);
        FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent);

        FileStream GetSingleDeleteProductFeedContent(AmazonListing amazonListing);
        FileStream GetSingleProductFeedContent(AmazonListing amazonListing);
        FileStream GetSingleProductPriceFeedContent(AmazonListing amazonListing);
        FileStream GetSingleProductInventoryFeedContent(AmazonListing amazonListing);
        FileStream GetSingleProductImageFeedContent(AmazonListing amazonListing);

        FileStream GetDeleteProductFeedsContent(AmazonListingGroup amazonListingGroup);
        FileStream GetProductFeedsContent(AmazonListingGroup amazonListingGroup);
        FileStream GetProductPriceFeedsContent(AmazonListingGroup amazonListingGroup);
        FileStream GetProductInventoryFeedsContent(AmazonListingGroup amazonListingGroup);
        FileStream GetProductImageFeedsContent(AmazonListingGroup amazonListingGroup);

        MarketplaceWebServiceProducts.Model.Product GetMatchingProductForId(string sku);

        IEnumerable<Order> ListOrders(AmazonSyncModel model);
        IEnumerable<Order> GetOrder(AmazonSyncModel model);
        IEnumerable<OrderItem> ListOrderItems(string amazonOrderId);
    }
}