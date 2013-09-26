using System.Collections.Generic;
using System.IO;
using MarketplaceWebService.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonFeedsApiService
    {
        FeedSubmissionInfo GetFeedSubmissionList(string submissionId);
        FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent);

        FileStream GetSingleProductDeleteFeed(AmazonListing listing);
        List<FileStream> GetSingleProductMainFeeds(AmazonListing listing);
        FileStream GetSingleProductImageFeed(AmazonListing listing);

        FileStream GetProductsDeleteFeeds(AmazonListingGroup amazonListingGroup);
        List<FileStream> GetProductsMainFeeds(AmazonListingGroup amazonListingGroup);
        FileStream GetProductsImageFeeds(AmazonListingGroup amazonListingGroup);
    }
}