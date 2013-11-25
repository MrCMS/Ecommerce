using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules
{
    public class ProductVariantIsNotNull : IDownloadProductBasicValidationRule
    {
        public DownloadProductResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            return productVariant == null ? new DownloadProductResult("Sorry, we cannot find a downloadable product here.") : null;
        }
    }

    public class UserIsLoggedIn : IDownloadProductBasicValidationRule
    {
        public DownloadProductResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var currentUser = CurrentRequestData.CurrentUser;

            return currentUser == null ? new DownloadProductResult("You are not authorized to download this content.") : null;
        }
    }

    public class GuidIsValid : IDownloadProductBasicValidationRule
    {
        public DownloadProductResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var guid = Guid.Empty;
            Guid.TryParse(oguid, out guid);

            if (string.IsNullOrWhiteSpace(oguid) || guid == Guid.Empty)
                return new DownloadProductResult("Please provide all necessary parameters in order to download content.");

            return null;
        }
    }

    public class OrderIsNotNull : IDownloadProductBasicValidationRule
    {
        private readonly IOrderService _orderService;

        public OrderIsNotNull(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public DownloadProductResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var guid = Guid.Empty;
            Guid.TryParse(oguid, out guid);

            var order = _orderService.GetByGuid(guid);

            return order == null ? new DownloadProductResult("Sorry, we cannot find a downloadable product here.") : null;
        }
    }

    public class LoggedUserIsAuthorizedToAccessOrder : IDownloadProductValidationRule
    {
        public DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            var currentUser = CurrentRequestData.CurrentUser;

            return order.User.Id != currentUser.Id ? new DownloadProductResult("You are not authorized to download this content.") : null;
        }
    }

    public class ProductVariantBelongsToOrder : IDownloadProductValidationRule
    {
        public DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return order.OrderLines.All(x => x.ProductVariant != productVariant) ? 
                new DownloadProductResult("Provided Order Id and Product Variant Id do not match.") : null;
        }
    }

    public class DownloadLinkIsNotTooOldForDownload : IDownloadProductValidationRule
    {
        public DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return (CurrentRequestData.Now-order.CreatedOn).Days > productVariant.Product.AllowedNumberOfDaysForDownload 
                ? new DownloadProductResult("Download link no longer active. Please contact the site owner for assistance.") : null;
        }
    }

    public class AllowedNumberOfDownloadsAvailable : IDownloadProductValidationRule
    {
        public DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return productVariant.Product.NumberOfDownloads >= productVariant.Product.AllowedNumberOfDownloads ? 
                new DownloadProductResult("Download link no longer active. Please contact the site owner for assistance.") : null;
        }
    }

    public class DownloadAvailable : IDownloadProductValidationRule
    {
        public DownloadProductResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return (string.IsNullOrWhiteSpace(type) && productVariant.Product.DownloadFileUrl != null)
                   || (!string.IsNullOrWhiteSpace(type) && type == "demo" && productVariant.Product.DemoFileUrl != null)
                       ? null
                       : new DownloadProductResult(
                             "Download link not found. Please contact the site owner for assistance.");
        }
    }
}