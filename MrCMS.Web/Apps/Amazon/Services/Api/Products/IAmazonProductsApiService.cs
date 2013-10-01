using MarketplaceWebServiceProducts.Model;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Products
{
    public interface IAmazonProductsApiService
    {
        AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection);
        Product GetMatchingProductForId(string sku);
    }
}