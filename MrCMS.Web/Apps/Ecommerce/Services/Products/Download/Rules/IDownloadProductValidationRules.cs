using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules
{
    public interface IDownloadProductValidationRule
    {
        DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "");
    }
    public interface IDownloadProductBasicValidationRule
    {
        DownloadProductResult GetErrors(string oguid, ProductVariant productVariant, string type = "");
    }
}