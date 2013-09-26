using System.Collections.Generic;
using System.IO;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Entities.Listings;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonGenerateFeedContentService
    {
        FileStream GetSingleFeed(object feed, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
                                 AmazonEnvelopeMessageOperationType amazonEnvelopeMessageOperationType);
        FileStream GetFeed(IEnumerable<object> feeds, AmazonEnvelopeMessageType amazonEnvelopeMessageType,
                           AmazonEnvelopeMessageOperationType amazonEnvelopeMessageOperationType);

        Product GetProductFeed(AmazonListing listing);

        Price GetProductPriceFeed(AmazonListing listing);

        Inventory GetProductInventoryFeed(AmazonListing listing);

        ProductImage GetProductImageFeed(AmazonListing listing);
    }
}