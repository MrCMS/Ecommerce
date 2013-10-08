using System;
using MarketplaceWebService;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceProducts;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public interface IAmazonApiService
    {
        MarketplaceWebServiceClient GetFeedsApiService();
        MarketplaceWebServiceProductsClient GetProductsApiService();
        MarketplaceWebServiceOrdersClient GetOrdersApiService();
    }
}