using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Website;
using Product = MarketplaceWebServiceFeedsClasses.Product;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public class AmazonFeedsApiService : IAmazonFeedsApiService
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonGenerateFeedService _amazonGenerateFeedContentService;

        public AmazonFeedsApiService(IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService, 
            IAmazonGenerateFeedService amazonGenerateFeedContentService, IAmazonApiService amazonApiService)
        {
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _amazonGenerateFeedContentService = amazonGenerateFeedContentService;
            _amazonApiService = amazonApiService;
        }

        public FeedSubmissionInfo GetFeedSubmissionList(string submissionId)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null,null,AmazonApiSection.Feeds, 
                    "GetFeedSubmissionList", null, null,null, "Getting result for Amazon Submission #"+ submissionId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = _amazonApiService.GetFeedsApiService();
                var request = _amazonApiService.GetFeedSubmissionListRequest(submissionId);

                var result = service.GetFeedSubmissionList(request);

                if (result != null && result.IsSetGetFeedSubmissionListResult()
                    && result.GetFeedSubmissionListResult != null)
                    return result.GetFeedSubmissionListResult.FeedSubmissionInfo.First();
            }
            catch (MarketplaceWebServiceException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Feeds, "GetFeedSubmissionList",null,null,null);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }

        public FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage,null,null,AmazonApiSection.Feeds,
                    "SubmitFeed",null,null,null,"Submitting "+feedType.GetDescription()+" to Amazon");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "SubmitFeed");
                var service = _amazonApiService.GetFeedsApiService();
                var request = _amazonApiService.GetSubmitFeedRequest(feedType, feedContent);

                var result = service.SubmitFeed(request);

                feedContent.Close();

                if (result != null && result.SubmitFeedResult != null && result.IsSetSubmitFeedResult() 
                    && result.SubmitFeedResult.FeedSubmissionInfo != null)
                    return result.SubmitFeedResult.FeedSubmissionInfo;
            }
            catch (MarketplaceWebServiceException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Feeds, "SubmitFeed",null,null,null);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }

        public FileStream GetSingleProductDeleteFeed(AmazonListing listing)
        {
            var product = new Product {SKU = listing.SellerSKU};
            return _amazonGenerateFeedContentService.GetSingleFeed(product, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Delete);
        }
        public List<FileStream> GetSingleProductMainFeeds(AmazonListing listing)
        {
            var feeds = new List<FileStream>
                {
                    _amazonGenerateFeedContentService.GetSingleFeed(
                        _amazonGenerateFeedContentService.GetProduct(listing),
                        AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Update),
                    _amazonGenerateFeedContentService.GetSingleFeed(
                        _amazonGenerateFeedContentService.GetProductPrice(listing),
                        AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update),
                    _amazonGenerateFeedContentService.GetSingleFeed(
                        _amazonGenerateFeedContentService.GetProductInventory(listing),
                        AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update)
                };
            return feeds;
        }
        public FileStream GetSingleProductImageFeed(AmazonListing listing)
        {
            return _amazonGenerateFeedContentService.GetSingleFeed(_amazonGenerateFeedContentService.GetProductImage(listing), AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        public FileStream GetProductsDeleteFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Where(x=>x.Status==AmazonListingStatus.Active).Select(x=>new Product{SKU = x.SellerSKU }).ToList();
            return _amazonGenerateFeedContentService.GetFeed(feeds, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Delete);
        }
        public List<FileStream> GetProductsMainFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feedCollection = new List<FileStream>();

            var products = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProduct).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(products, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Update));

            var prices = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProductPrice).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(prices, AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update));

            var inventories = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProductInventory).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(inventories, AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update));

            return feedCollection;
        }
        public FileStream GetProductsImageFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Where(x => x.Status == AmazonListingStatus.NotOnAmazon
                || x.Status == AmazonListingStatus.Inactive).Select(_amazonGenerateFeedContentService.GetProductImage).ToList();
            return _amazonGenerateFeedContentService.GetFeed(feeds, AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        public FileStream GetOrderFulfillmentFeed(AmazonOrder order)
        {
            return _amazonGenerateFeedContentService.GetSingleFeed(_amazonGenerateFeedContentService.GetOrderFulfillment(order),
                AmazonEnvelopeMessageType.OrderFulfillment, null);
        }
        public FileStream GetOrderFulfillmentFeed(IEnumerable<AmazonOrder> orders)
        {
            var feeds = orders.Select(_amazonGenerateFeedContentService.GetOrderFulfillment).ToList();
            return _amazonGenerateFeedContentService.GetFeed(feeds, AmazonEnvelopeMessageType.OrderFulfillment,null);
        }
    }
}