using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using Product = MarketplaceWebServiceFeedsClasses.Product;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public class AmazonGenerateFeedContentService : IAmazonGenerateFeedContentService
    {
        private readonly AmazonSellerSettings _amazonSellerSettings;

        public AmazonGenerateFeedContentService(AmazonSellerSettings amazonSellerSettings)
        {
            _amazonSellerSettings = amazonSellerSettings;
        }

        public FileStream GetSingleFeed(object feed, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
       AmazonEnvelopeMessageOperationType? amazonEnvelopeMessageOperationType)
        {
            if (feed != null)
            {
                var message = new AmazonEnvelopeMessage
                    {
                        MessageID = "1",
                        Item = feed
                    };
                if (amazonEnvelopeMessageOperationType != null)
                    message.OperationType = amazonEnvelopeMessageOperationType.Value;
                var amazonEnvelope = new AmazonEnvelope
                {
                    Header = new Header
                    {
                        DocumentVersion = "1.0",
                        MerchantIdentifier = _amazonSellerSettings.SellerId
                    },
                    MessageType = amazonEnvelopeMessageType,
                    Message = new AmazonEnvelopeMessageCollection(){message}
                };

                return AmazonAppHelper.GetStreamFromAmazonEnvelope(amazonEnvelope, amazonEnvelopeMessageType);
            }
            return null;
        }

        public FileStream GetFeed(IEnumerable<object> feeds, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
            AmazonEnvelopeMessageOperationType amazonEnvelopeMessageOperationType)
        {
            if (feeds != null && feeds.Any())
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
            return null;
        }

        public Product GetProductFeed(AmazonListing listing)
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

        public Price GetProductPriceFeed(AmazonListing listing)
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

        public Inventory GetProductInventoryFeed(AmazonListing listing)
        {
            var inventory=new Inventory
            {
                SKU = listing.SellerSKU,
                SwitchFulfillmentTo = listing.FulfillmentChannel.HasValue ? 
                listing.FulfillmentChannel.Value.GetEnumByValue<InventorySwitchFulfillmentTo>()
                :InventorySwitchFulfillmentTo.MFN
            };
            if (inventory.SwitchFulfillmentTo == InventorySwitchFulfillmentTo.AFN)
            {
                inventory.FulfillmentCenterID = _amazonSellerSettings.DefaultFulfillmentCenter;
                inventory.Item = InventoryLookup.FulfillmentNetwork;
            }
            else
            {
                inventory.FulfillmentCenterID = "DEFAULT";
                inventory.Item = listing.Quantity.ToString();
            }
            return inventory;
        }

        public ProductImage GetProductImageFeed(AmazonListing listing)
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

        public OrderAcknowledgement GetOrderAcknowledgmentFeed(AmazonOrder amazonOrder, OrderAcknowledgementStatusCode orderAcknowledgementStatusCode, 
            OrderAcknowledgementItemCancelReason? orderAcknowledgementItemCancelReason)
        {
            var product = new OrderAcknowledgement
            {
                AmazonOrderID = amazonOrder.AmazonOrderId,
                StatusCode = orderAcknowledgementStatusCode,
                Item=new OrderAcknowledgementItemCollection()
            };
            foreach (var amazonOrderItem in amazonOrder.Items)
            {
                var item = new OrderAcknowledgementItem() {AmazonOrderItemCode = amazonOrderItem.AmazonOrderItemId,CancelReasonSpecified = true};
                if (orderAcknowledgementItemCancelReason.HasValue)
                    item.CancelReason = orderAcknowledgementItemCancelReason.Value;
                product.Item.Add(item);
            }
            return product;
        }

        public OrderFulfillment GetOrderFulfillmentFeed(AmazonOrder amazonOrder)
        {
            var orderFulfillment = new OrderFulfillment()
            {
                Item=amazonOrder.AmazonOrderId,
                ItemElementName = ItemChoiceType2.AmazonOrderID,
                FulfillmentDate = amazonOrder.Order.ShippingDate??CurrentRequestData.Now.AddHours(-8),
                FulfillmentData = new OrderFulfillmentFulfillmentData()
                    {
                        Item="Other",
                        ShippingMethod = "Standard"
                    },
               Item1 = new OrderFulfillmentItemCollection()
            };
            foreach (var amazonOrderItem in amazonOrder.Items)
            {
                var item = new OrderFulfillmentItem()
                    {
                        Item = amazonOrderItem.AmazonOrderItemId,
                        ItemElementName = ItemChoiceType3.AmazonOrderItemCode,
                        Quantity = decimal.ToInt32(amazonOrderItem.QuantityOrdered).ToString()
                    };
                orderFulfillment.Item1.Add(item);
            }
            return orderFulfillment;
        }
    }
}