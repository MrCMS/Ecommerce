using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceFeedsClasses;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

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

        private MarketplaceWebServiceClient GetFeedsApiService()
        {
            var config = new MarketplaceWebServiceConfig() { ServiceURL = _amazonAppSettings.ApiEndpoint };
            return new MarketplaceWebServiceClient(MrCMSApplication.Get<AmazonAppSettings>().AWSAccessKeyId,
                                                    MrCMSApplication.Get<AmazonAppSettings>().SecretKey, "MrCMS", MrCMSApplication.AssemblyVersion,
                                                    config);
        }
        private MarketplaceWebServiceOrdersClient GetOrdersApiService()
        {
            var config = new MarketplaceWebServiceOrdersConfig() { ServiceURL = _amazonAppSettings.OrdersApiEndpoint };
            return new MarketplaceWebServiceOrdersClient("MrCMS", MrCMSApplication.AssemblyVersion, _amazonAppSettings.AWSAccessKeyId,
                                                         _amazonAppSettings.SecretKey, config);
        }
        public ServiceStatusEnum GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");
                switch (apiSection)
                {
                    case AmazonApiSection.Orders:
                        var service = GetOrdersApiService();
                        var request = new GetServiceStatusRequest {SellerId = _amazonSellerSettings.SellerId};
                        var result = service.GetServiceStatus(request);
                        if (result != null && result.GetServiceStatusResult != null)
                            return result.GetServiceStatusResult.Status;
                        break;
                    default:
                        return ServiceStatusEnum.RED;
                }
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus");
                return ServiceStatusEnum.RED;
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return ServiceStatusEnum.RED;
            }
            return ServiceStatusEnum.RED;
        }

        #endregion

        #region Listings
        public FeedSubmissionInfo GetFeedSubmissionList(string submissionId)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "GetFeedSubmissionList");
                var service = GetFeedsApiService();
                var request = new GetFeedSubmissionListRequest()
                {
                    Merchant = _amazonSellerSettings.SellerId,   
                    FeedSubmissionIdList = new IdList() { Id = new List<string>(){submissionId}}
                };
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
        public FeedSubmissionInfo SubmitFeed(AmazonFeedType feedType, FileStream feedContent)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Feeds, "SubmitFeed");
                var service = GetFeedsApiService();
                var request = new SubmitFeedRequest()
                {
                    Merchant = _amazonSellerSettings.SellerId,
                    ContentType = new ContentType(MediaType.XML),
                    FeedContent = feedContent,
                    FeedType = feedType.ToString(),
                    MarketplaceIdList = new IdList { Id = new List<string>(new[] { _amazonSellerSettings.MarketplaceId }) }
                };
                request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);

                var result = service.SubmitFeed(request);
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
        public FileStream GetProductFeedContent(AmazonListing listing)
        {
            return GetFeed(GetProductFeed(listing), AmazonEnvelopeMessageType.Product);
        }
        public FileStream GetProductPriceFeedContent(AmazonListing listing)
        {
            return GetFeed(GetProductPriceFeed(listing), AmazonEnvelopeMessageType.Price);
        }
        public FileStream GetProductInventoryFeedContent(AmazonListing listing)
        {
            return GetFeed(GetProductInventoryFeed(listing), AmazonEnvelopeMessageType.Inventory);
        }
        public FileStream GetProductImageFeedContent(AmazonListing listing)
        {
            return GetFeed(GetProductImageFeed(listing), AmazonEnvelopeMessageType.ProductImage);
        }
        private FileStream GetFeed(object feed, AmazonEnvelopeMessageType messageType)
        {
            var amazonEnvelope = new AmazonEnvelope
                {
                    Header = new Header
                        {
                            DocumentVersion = "1.0",
                            MerchantIdentifier = _amazonSellerSettings.SellerId
                        },
                    MessageType = messageType,
                    Message = new AmazonEnvelopeMessageCollection()
                        {
                            new AmazonEnvelopeMessage {MessageID = "1", Item = feed}
                        }
                };
            var xmlString = AmazonApiHelper.Serialize(amazonEnvelope);
            var fileLocation = AmazonApiHelper.GetAmazonApiFolderPath(string.Format("{0}/{1}/{2}",CurrentRequestData.CurrentSite.Id, "amazon",string.Format("Amazon{0}Feed",messageType) +"-"+CurrentRequestData.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".xml"));
            File.WriteAllText(fileLocation, xmlString);
            return File.Open(fileLocation, FileMode.Open, FileAccess.Read);
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
            var productImages = new List<ProductImage>();
            //if (listing.ProductVariant!=null && listing.ProductVariant.Product.Images.Any())
            //{
            //    var image = listing.ProductVariant.Product.Images.First();
            //    if(image.FileExtension.Contains(".jpeg"))
            //    {
            //        productImages.Add(new ProductImage()
            //        {
            //            SKU = listing.SellerSKU,
            //            ImageType = ProductImageImageType.Main,
            //            ImageLocation = AmazonApiHelper.GenerateImageUrl(image.FileUrl)
            //        });
            //    }
            //}
            return new ProductImage()
                {
                    SKU = listing.SellerSKU,
                    ImageType = ProductImageImageType.Main,
                    ImageLocation =
                        "https://www.ryness.co.uk/images/thumbs/0005659_300.jpeg"
                };
        }
        #endregion

        #region Orders

        public IEnumerable<Order> GetOrder(AmazonSyncModel model)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "GetOrder");
                var service = GetOrdersApiService();
                var request = new GetOrderRequest()
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = new OrderIdList().WithId(model.Description)
                };
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
        public IEnumerable<Order> ListOrders(AmazonSyncModel model)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
                var service = GetOrdersApiService();
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
        public IEnumerable<OrderItem> ListOrderItems(string amazonOrderId)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrderItems");
                var service = GetOrdersApiService();
                var request = new ListOrderItemsRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = amazonOrderId
                };
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

        #endregion
    }
}