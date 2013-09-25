using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceFeedsClasses;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MarketplaceWebServiceProducts;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using GetServiceStatusRequest = MarketplaceWebServiceOrders.Model.GetServiceStatusRequest;
using Product = MarketplaceWebServiceFeedsClasses.Product;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public class AmazonApiService : IAmazonApiService
    {
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonApiService(AmazonAppSettings amazonAppSettings, 
            AmazonSellerSettings amazonSellerSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService)
        {
            _amazonAppSettings = amazonAppSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
        }

        #region Api

        public AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");
                switch (apiSection)
                {
                    case AmazonApiSection.Orders:
                        var ordersApiService = GetOrdersApiService();
                        var ordersApiRequest = new GetServiceStatusRequest { SellerId = _amazonSellerSettings.SellerId };
                        var ordersApiResult = ordersApiService.GetServiceStatus(ordersApiRequest);
                        if (ordersApiResult != null && ordersApiResult.GetServiceStatusResult != null)
                            return ordersApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();
                        break;
                    case AmazonApiSection.Products:
                        var productsApiService = GetProductsApiService();
                        var productsApiRequest = new MarketplaceWebServiceProducts.Model.GetServiceStatusRequest { SellerId = _amazonSellerSettings.SellerId };
                        var productsApiResult = productsApiService.GetServiceStatus(productsApiRequest);
                        if (productsApiResult != null && productsApiResult.GetServiceStatusResult != null)
                            return productsApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();
                        break;
                    default:
                        return AmazonServiceStatus.RED;
                }
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return AmazonServiceStatus.RED;
        }

        private MarketplaceWebServiceClient GetFeedsApiService()
        {
            var config = new MarketplaceWebServiceConfig() { ServiceURL = _amazonAppSettings.ApiEndpoint };
            return new MarketplaceWebServiceClient(MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey, "MrCMS", MrCMSApplication.AssemblyVersion,
                                                    config);
        }
        private MarketplaceWebServiceProductsClient GetProductsApiService()
        {
            var config = new MarketplaceWebServiceProductsConfig() { ServiceURL = _amazonAppSettings.ProductsApiEndpoint };
            return new MarketplaceWebServiceProductsClient("MrCMS", MrCMSApplication.AssemblyVersion,MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey,
                                                    config);
        }
        private MarketplaceWebServiceOrdersClient GetOrdersApiService()
        {
            var config = new MarketplaceWebServiceOrdersConfig() { ServiceURL = _amazonAppSettings.OrdersApiEndpoint };
            return new MarketplaceWebServiceOrdersClient("MrCMS", MrCMSApplication.AssemblyVersion, _amazonAppSettings.AWSAccessKeyId,
                                                         _amazonAppSettings.SecretKey, config);
        }

        #endregion

        #region Feeds

        public FeedSubmissionInfo GetFeedSubmissionList(string submissionId)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Feeds, null, null, "GetFeedSubmissionList", "Getting Result for Amazon Submission #"+ submissionId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = GetFeedsApiService();
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
            var request = new GetFeedSubmissionListRequest()
                {
                    Merchant = _amazonSellerSettings.SellerId,
                    FeedSubmissionIdList = new IdList() {Id = new List<string>() {submissionId}}
                };
            return request;
        }

        public FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage,AmazonApiSection.Feeds,null,null,"SubmitFeed","Submitting "+feedType+" Feed to Amazon");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "SubmitFeed");
                var service = GetFeedsApiService();
                var request = GetSubmitFeedRequest(feedType, feedContent);

                var result = service.SubmitFeed(request);

                feedContent.Close();

                if (result != null && result.SubmitFeedResult != null && result.IsSetSubmitFeedResult() && result.SubmitFeedResult.FeedSubmissionInfo != null)
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

        public FileStream GetSingleDeleteProductFeedContent(AmazonListing listing)
        {
            var product = new Product {SKU = listing.SellerSKU};
            return GetSingleFeed(product, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Delete);
        }
        public FileStream GetSingleProductFeedContent(AmazonListing listing)
        {
            return GetSingleFeed(GetProductFeed(listing), AmazonEnvelopeMessageType.Product,AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetSingleProductPriceFeedContent(AmazonListing listing)
        {
            return GetSingleFeed(GetProductPriceFeed(listing), AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetSingleProductInventoryFeedContent(AmazonListing listing)
        {
            return GetSingleFeed(GetProductInventoryFeed(listing), AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetSingleProductImageFeedContent(AmazonListing listing)
        {
            return GetSingleFeed(GetProductImageFeed(listing), AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        public FileStream GetDeleteProductFeedsContent(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Where(x=>x.Status==AmazonListingStatus.Active).Select(x=>new Product{SKU = x.SellerSKU }).ToList();
            return GetFeed(feeds, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Delete);
        }
        public FileStream GetProductFeedsContent(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Select(GetProductFeed).ToList();
            return GetFeed(feeds, AmazonEnvelopeMessageType.Product, AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetProductPriceFeedsContent(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Select(GetProductPriceFeed).ToList();
            return GetFeed(feeds, AmazonEnvelopeMessageType.Price, AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetProductInventoryFeedsContent(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Select(GetProductInventoryFeed).ToList();
            return GetFeed(feeds, AmazonEnvelopeMessageType.Inventory, AmazonEnvelopeMessageOperationType.Update);
        }
        public FileStream GetProductImageFeedsContent(AmazonListingGroup amazonListingGroup)
        {
            var feeds = amazonListingGroup.Items.Select(GetProductImageFeed).ToList();
            return GetFeed(feeds, AmazonEnvelopeMessageType.ProductImage, AmazonEnvelopeMessageOperationType.Update);
        }

        private FileStream GetSingleFeed(object feed, AmazonEnvelopeMessageType amazonEnvelopeMessageType, AmazonEnvelopeMessageOperationType amazonEnvelopeMessageOperationType)
        {
            if (feed != null)
            {
                var amazonEnvelope = new AmazonEnvelope
                    {
                        Header = new Header
                            {
                                DocumentVersion = "1.0",
                                MerchantIdentifier = _amazonSellerSettings.SellerId
                            },
                        MessageType = amazonEnvelopeMessageType,
                        Message =
                            new AmazonEnvelopeMessageCollection()
                                {
                                    new AmazonEnvelopeMessage
                                        {
                                            MessageID = "1",
                                            Item = feed,
                                            OperationType = amazonEnvelopeMessageOperationType
                                        }
                                }
                    };

                return AmazonAppHelper.GetStreamFromAmazonEnvelope(amazonEnvelope, amazonEnvelopeMessageType);
            }
            return null;
        }
        private FileStream GetFeed(IEnumerable<object> feeds, AmazonEnvelopeMessageType amazonEnvelopeMessageType, AmazonEnvelopeMessageOperationType amazonEnvelopeMessageOperationType)
        {
            var messages = new AmazonEnvelopeMessageCollection();
            var msgCounter = 1;
            foreach (var feed in feeds)
            {
                messages.Add(new AmazonEnvelopeMessage
                    {
                        MessageID = msgCounter.ToString(), 
                        OperationType = amazonEnvelopeMessageOperationType,
                        Item = feed
                    });
                msgCounter++;
            }
            var amazonEnvelope = new AmazonEnvelope
            {
                Header = new Header
                {
                    DocumentVersion = "1.0",
                    MerchantIdentifier = _amazonSellerSettings.SellerId
                },
                MessageType = amazonEnvelopeMessageType,
                Message = messages
            };

            return AmazonAppHelper.GetStreamFromAmazonEnvelope(amazonEnvelope, amazonEnvelopeMessageType);
        }

        private Product GetProductFeed(AmazonListing listing)
        {
            var product=new Product
            {
                Condition = new ConditionInfo { ConditionType = listing.Condition, ConditionNote = listing.ConditionNote},
                SKU = listing.SellerSKU,
                StandardProductID = new StandardProductID()
                {
                    Type = listing.StandardProductIDType,
                    Value = listing.StandardProductId
                },
                DescriptionData = new ProductDescriptionData()
                {
                    Brand = listing.Brand,
                    Title = listing.Title,
                    Manufacturer = listing.Manafacturer,
                    MfrPartNumber = listing.MfrPartNumber
                }
            };
            if (listing.ReleaseDate.HasValue)
                product.ReleaseDate = listing.ReleaseDate.Value;
            return product;
        }
        private Price GetProductPriceFeed(AmazonListing listing)
        {
            return new Price
            {
                SKU = listing.SellerSKU,
                StandardPrice = new OverrideCurrencyAmount()
                    {
                        Currency = BaseCurrencyCodeWithDefault.USD,
                        Value = listing.Price
                    }
            };
        }
        private Inventory GetProductInventoryFeed(AmazonListing listing)
        {
            var inve=new Inventory
            {
                SKU = listing.SellerSKU,
                Item = listing.Quantity.ToString()
            };
            return inve;
        }
        private ProductImage GetProductImageFeed(AmazonListing listing)
        {
            if (listing.ProductVariant != null && listing.ProductVariant.Product.Images.Any())
            {
                var image = listing.ProductVariant.Product.Images.First();
                if (image.FileExtension.Contains(".jpeg"))
                {
                    return new ProductImage()
                    {
                        SKU = listing.SellerSKU,
                        ImageType = ProductImageImageType.Main,
                        ImageLocation = AmazonAppHelper.GenerateImageUrl(image.FileUrl)
                    };
                }
            }
            return null;
        }

        #endregion

        #region Products

        public MarketplaceWebServiceProducts.Model.Product GetMatchingProductForId(string sku)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Products, null, null, "GetMatchingProductForId", "Get Matching Product For SKU #" + sku);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = GetProductsApiService();
                var request = GetMatchingProductForIdRequest(sku);

                var result = service.GetMatchingProductForId(request);

                if (result != null && result.IsSetGetMatchingProductForIdResult()
                    && result.GetMatchingProductForIdResult != null && result.GetMatchingProductForIdResult.Any()
                    && result.GetMatchingProductForIdResult.First().status=="Success"
                    && result.GetMatchingProductForIdResult.First().Products.Product.Any())
                    return result.GetMatchingProductForIdResult.First().Products.Product.First();
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
        private GetMatchingProductForIdRequest GetMatchingProductForIdRequest(string sku)
        {
            var request = new GetMatchingProductForIdRequest()
            {
                IdType = "SellerSKU",
                IdList = new IdListType().WithId(sku),
                MarketplaceId = _amazonSellerSettings.MarketplaceId,
                SellerId = _amazonSellerSettings.SellerId
            };
            return request;
        }

        #endregion

        #region Orders

        public IEnumerable<Order> GetOrder(AmazonSyncModel model)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, "GetOrder", "Getting Amazon Orders");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "GetOrder");
                var service = GetOrdersApiService();
                var request = GetOrderRequest(model);

                var result = service.GetOrder(request);

                if (result != null && result.GetOrderResult != null && result.IsSetGetOrderResult() && result.GetOrderResult.Orders.Order != null)
                    return result.GetOrderResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders, "GetOrder");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private GetOrderRequest GetOrderRequest(AmazonSyncModel model)
        {
            var request = new GetOrderRequest()
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = new OrderIdList().WithId(model.Description)
                };
            return request;
        }

        public IEnumerable<Order> ListOrders(AmazonSyncModel model)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, "ListOrders", "Listing Amazon Orders");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
                var service = GetOrdersApiService();
                var request = GetListOrdersRequest(model);

                var result = service.ListOrders(request);

                if (result != null && result.ListOrdersResult != null && result.IsSetListOrdersResult() && result.ListOrdersResult.Orders.Order != null)
                    return result.ListOrdersResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders, "ListOrders");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private ListOrdersRequest GetListOrdersRequest(AmazonSyncModel model)
        {
            var marketPlace = new MarketplaceIdList();
            var request = new ListOrdersRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    MarketplaceId = marketPlace.WithId(_amazonSellerSettings.MarketplaceId),
                };
            if (model.From.HasValue)
                request.CreatedAfter = model.From.Value;
            if (model.To.HasValue)
                request.CreatedBefore = model.To.Value;
            return request;
        }

        public IEnumerable<OrderItem> ListOrderItems(string amazonOrderId)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, "ListOrderItems", "Listing Items for Amazon Order #"+amazonOrderId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrderItems");
                var service = GetOrdersApiService();
                var request = GetListOrderItemsRequest(amazonOrderId);

                var result = service.ListOrderItems(request);

                if (result != null && result.ListOrderItemsResult != null && result.IsSetListOrderItemsResult() 
                    && result.ListOrderItemsResult.OrderItems != null)
                    return result.ListOrderItemsResult.OrderItems.OrderItem;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders, "ListOrderItems");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private ListOrderItemsRequest GetListOrderItemsRequest(string amazonOrderId)
        {
            var request = new ListOrderItemsRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = amazonOrderId
                };
            return request;
        }

        #endregion
    }
}