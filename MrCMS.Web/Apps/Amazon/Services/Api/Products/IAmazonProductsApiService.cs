using MarketplaceWebServiceProducts.Model;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Products
{
    public interface IAmazonProductsApiService
    {
        Product GetMatchingProductForId(string sku);
    }
}