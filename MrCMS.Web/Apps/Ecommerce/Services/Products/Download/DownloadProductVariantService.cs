using System.Text;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System;
using System.Web.Mvc;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public interface IDownloadProductVariantService
    {
        FilePathResult Download(ProductVariant productVariant, Entities.Orders.Order order, string type = "");
        DownloadProductVariantResult Validate(ref Entities.Orders.Order order, string oguid, ProductVariant productVariant, string type = "");
    }
    public class DownloadProductVariantService : IDownloadProductVariantService
    {
        private readonly IOrderService _orderService;
        private readonly IProductVariantService _productService;

        public DownloadProductVariantService(IOrderService orderService, IProductVariantService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public FilePathResult Download(ProductVariant productVariant, Entities.Orders.Order order, string type = "")
        {
            productVariant.NumberOfDownloads++;
            _productService.Update(productVariant);

            if (string.IsNullOrWhiteSpace(type) && productVariant.DownloadFileUrl != null)
            {
                return new FilePathResult(productVariant.DownloadFile.FileUrl,
                    productVariant.DownloadFile.ContentType) 
                    { FileDownloadName = productVariant.DownloadFile.FileName };
            }
            return new FilePathResult(productVariant.DemoFile.FileUrl, 
                productVariant.DemoFile.ContentType) 
                { FileDownloadName = productVariant.DemoFile.FileName };
        }

        public DownloadProductVariantResult Validate(ref Entities.Orders.Order order,string oguid, ProductVariant productVariant, string type = "")
        {
            var basicRules = MrCMSApplication.GetAll<IDownloadProductBasicValidationRule>().ToList();
            var rules = MrCMSApplication.GetAll<IDownloadProductValidationRule>().ToList();

            foreach (var result in basicRules.Select(rule => rule.
                GetErrors(oguid, productVariant, type)).Where(result => result != null))
            {
                return result;
            }

            order = _orderService.GetByGuid(Guid.Parse(oguid));
            var stableOrder = order;

            foreach (var result in rules.Select(rule => rule.
               GetErrors(stableOrder, productVariant, type)).Where(result => result != null))
            {
                return result;
            }

            return null;
        }
    }

    public class DownloadProductVariantResult : ContentResult
    {
        public DownloadProductVariantResult(string message)
        {
            Content = message;
            ContentEncoding = Encoding.UTF8;
            ContentType = "text/plain";
        }
    }
}