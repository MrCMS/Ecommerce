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
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using Product = MarketplaceWebServiceFeedsClasses.Product;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public class AmazonFeedsApiService : IAmazonFeedsApiService
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonGenerateFeedContentService _amazonGenerateFeedContentService;

        public AmazonFeedsApiService(AmazonSellerSettings amazonSellerSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService, 
            IAmazonGenerateFeedContentService amazonGenerateFeedContentService, IAmazonApiService amazonApiService)
        {
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _amazonGenerateFeedContentService = amazonGenerateFeedContentService;
            _amazonApiService = amazonApiService;
        }

        public FeedSubmissionInfo GetFeedSubmissionList(string submissionId)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Feeds, 
                    null, null, "GetFeedSubmissionList", "Getting result for Amazon Submission #"+ submissionId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = _amazonApiService.GetFeedsApiService();
                var request = GetFeedSubmissionListRequest(submissionId);

                var result = service.GetFeedSubmissionList(request);

                if (result != null && result.IsSetGetFeedSubmissionListResult()
                    && result.GetFeedSubmissionListResult != null)
                    return result.GetFeedSubmissionListResult.FeedSubmissionInfo.First();
            }
            catch (MarketplaceWebServiceException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Feeds, "GetFeedSubmissionList");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private GetFeedSubmissionListRequest GetFeedSubmissionListRequest(string submissionId)
        {
            return new GetFeedSubmissionListRequest()
                {
                    Merchant = _amazonSellerSettings.SellerId,
                    FeedSubmissionIdList = new IdList() {Id = new List<string>() {submissionId}}
                };
        }

        public FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage,AmazonApiSection.Feeds,
                    null,null,"SubmitFeed","Submitting "+feedType+" Feed to Amazon");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "SubmitFeed");
                var service = _amazonApiService.GetFeedsApiService();
                var request = GetSubmitFeedRequest(feedType, feedContent);

                var result = service.SubmitFeed(request);

                feedContent.Close();

                if (result != null && result.SubmitFeedResult != null && result.IsSetSubmitFeedResult() 
                    && result.SubmitFeedResult.FeedSubmissionInfo != null)
                    return result.SubmitFeedResult.FeedSubmissionInfo;
            }
            catch (MarketplaceWebServiceException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Feeds, "SubmitFeed");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private SubmitFeedRequest GetSubmitFeedRequest(AmazonFeedType feedType, FileStream feedContent)
        {
            var request = new SubmitFeedRequest()
                {
                    Merchant = _amazonSellerSettings.SellerId,
                    ContentType = new ContentType(MediaType.XML),
                    FeedContent = feedContent,
                    FeedType = feedType.ToString(),
                    MarketplaceIdList = new IdList {Id = new List<string>(new[] {_amazonSellerSettings.MarketplaceId})}
                };
            request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
            return request;
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
                        _amazonGenerateFeedContentService.GetProductFeed(listing),
                        AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Update),
                    _amazonGenerateFeedContentService.GetSingleFeed(
                        _amazonGenerateFeedContentService.GetProductPriceFeed(listing),
                        AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update),
                    _amazonGenerateFeedContentService.GetSingleFeed(
                        _amazonGenerateFeedContentService.GetProductInventoryFeed(listing),
                        AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update)
                };
            return feeds;
        }

        public FileStream GetSingleProductImageFeed(AmazonListing listing)
        {
            return _amazonGenerateFeedContentService.GetSingleFeed(_amazonGenerateFeedContentService.GetProductImageFeed(listing), AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        public FileStream GetProductsDeleteFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Where(x=>x.Status==AmazonListingStatus.Active).Select(x=>new Product{SKU = x.SellerSKU }).ToList();
            return _amazonGenerateFeedContentService.GetFeed(feeds, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Delete);
        }
        public List<FileStream> GetProductsMainFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feedCollection = new List<FileStream>();

            var products = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProductFeed).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(products, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Update));

            var prices = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProductPriceFeed).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(prices, AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update));

            var inventories = amazonListingGroup.Items.Select(_amazonGenerateFeedContentService.GetProductInventoryFeed).ToList();
            feedCollection.Add(_amazonGenerateFeedContentService.GetFeed(inventories, AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update));

            return feedCollection;
        }
        public FileStream GetProductsImageFeeds(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Where(x => x.Status == AmazonListingStatus.NotOnAmazon
                || x.Status == AmazonListingStatus.Inactive).Select(_amazonGenerateFeedContentService.GetProductImageFeed).ToList();
            return _amazonGenerateFeedContentService.GetFeed(feeds, AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        public FileStream GetOrderAcknowledgmentFeed(AmazonOrder order,OrderAcknowledgementStatusCode orderAcknowledgementStatusCode)
        {
            return _amazonGenerateFeedContentService.GetSingleFeed(_amazonGenerateFeedContentService.GetOrderAcknowledgmentFeed(order, orderAcknowledgementStatusCode,order.CancelReason), 
                AmazonEnvelopeMessageType.OrderAcknowledgement, null);
        }

        public FileStream GetOrderFulfillmentFeed(AmazonOrder order)
        {
            return _amazonGenerateFeedContentService.GetSingleFeed(_amazonGenerateFeedContentService.GetOrderFulfillmentFeed(order),
                AmazonEnvelopeMessageType.OrderFulfillment, null);
        }
    }
}