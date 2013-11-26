using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DownloadProductController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IDownloadProductVariantService _downloadProductService;

        public DownloadProductController(IDownloadProductVariantService downloadProductService)
        {
            _downloadProductService = downloadProductService;
        }

        public ActionResult Download(string oguid, ProductVariant productVariant, string type="")
        {
            Order order = null;
            var result = _downloadProductService.Validate(ref order,oguid, productVariant, type);
            return (ActionResult) result ?? _downloadProductService.Download(productVariant, order, type);
        }
    }
}
