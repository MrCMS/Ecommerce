using System.Collections.Generic;
using System.IO;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonGenerateFeedService
    {
        FileStream GetSingleFeed(object feed, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
                                 AmazonEnvelopeMessageOperationType? amazonEnvelopeMessageOperationType);
        FileStream GetFeed(IEnumerable<object> feeds, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
                           AmazonEnvelopeMessageOperationType? amazonEnvelopeMessageOperationType);

        Product GetProduct(AmazonListing listing);
        Price GetProductPrice(AmazonListing listing);
        Inventory GetProductInventory(AmazonListing listing);
        ProductImage GetProductImage(AmazonListing listing);

        OrderFulfillment GetOrderFulfillment(AmazonOrder amazonOrder);
    }
}