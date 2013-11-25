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
    public interface IDownloadProductService
    {
        FilePathResult Download(ProductVariant productVariant, Entities.Orders.Order order, string type = "");
        DownloadProductResult Validate(ref Entities.Orders.Order order, string oguid, ProductVariant productVariant, string type = "");
    }
    public class DownloadProductService : IDownloadProductService
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public DownloadProductService(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public FilePathResult Download(ProductVariant productVariant, Entities.Orders.Order order, string type = "")
        {
            productVariant.Product.NumberOfDownloads++;
            _productService.Update(productVariant.Product);

            if (string.IsNullOrWhiteSpace(type) && productVariant.Product.DownloadFileUrl != null)
            {
                return new FilePathResult(productVariant.Product.DownloadFile.FileUrl,
                    productVariant.Product.DownloadFile.ContentType) 
                    { FileDownloadName = productVariant.Product.DownloadFile.FileName };
            }
            return new FilePathResult(productVariant.Product.DemoFile.FileUrl, 
                productVariant.Product.DemoFile.ContentType) 
                { FileDownloadName = productVariant.Product.DemoFile.FileName };
        }

        public DownloadProductResult Validate(ref Entities.Orders.Order order,string oguid, ProductVariant productVariant, string type = "")
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

    public class DownloadProductResult : ContentResult
    {
        public DownloadProductResult(string message)
        {
            Content = message;
            ContentEncoding = Encoding.UTF8;
            ContentType = "text/plain";
        }
    }
}