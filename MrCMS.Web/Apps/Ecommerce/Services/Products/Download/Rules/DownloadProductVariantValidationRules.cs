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
        public DownloadProductVariantResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            return productVariant == null ? new DownloadProductVariantResult("Sorry, we cannot find a downloadable product here.") : null;
        }
    }

    public class UserIsLoggedIn : IDownloadProductBasicValidationRule
    {
        public DownloadProductVariantResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var currentUser = CurrentRequestData.CurrentUser;

            return currentUser == null ? new DownloadProductVariantResult("You are not authorized to download this content.") : null;
        }
    }

    public class GuidIsValid : IDownloadProductBasicValidationRule
    {
        public DownloadProductVariantResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var guid = Guid.Empty;
            Guid.TryParse(oguid, out guid);

            if (string.IsNullOrWhiteSpace(oguid) || guid == Guid.Empty)
                return new DownloadProductVariantResult("Please provide all necessary parameters in order to download content.");

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

        public DownloadProductVariantResult GetErrors(string oguid, ProductVariant productVariant, string type = "")
        {
            var guid = Guid.Empty;
            Guid.TryParse(oguid, out guid);

            var order = _orderService.GetByGuid(guid);

            return order == null ? new DownloadProductVariantResult("Sorry, we cannot find a downloadable product here.") : null;
        }
    }

    public class LoggedUserIsAuthorizedToAccessOrder : IDownloadProductValidationRule
    {
        public DownloadProductVariantResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            var currentUser = CurrentRequestData.CurrentUser;

            return order.User.Id != currentUser.Id ? new DownloadProductVariantResult("You are not authorized to download this content.") : null;
        }
    }

    public class ProductVariantBelongsToOrder : IDownloadProductValidationRule
    {
        public DownloadProductVariantResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return order.OrderLines.All(x => x.ProductVariant != productVariant) ? 
                new DownloadProductVariantResult("Provided Order Id and Product Variant Id do not match.") : null;
        }
    }

    public class DownloadLinkIsNotTooOldForDownload : IDownloadProductValidationRule
    {
        public DownloadProductVariantResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return (CurrentRequestData.Now-order.CreatedOn).Days > productVariant.AllowedNumberOfDaysForDownload 
                ? new DownloadProductVariantResult("Download link no longer active. Please contact the site owner for assistance.") : null;
        }
    }

    public class AllowedNumberOfDownloadsAvailable : IDownloadProductValidationRule
    {
        public DownloadProductVariantResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return productVariant.NumberOfDownloads >= productVariant.AllowedNumberOfDownloads ? 
                new DownloadProductVariantResult("Download link no longer active. Please contact the site owner for assistance.") : null;
        }
    }

    public class DownloadAvailable : IDownloadProductValidationRule
    {
        public DownloadProductVariantResult GetErrors(Order order, ProductVariant productVariant, string type = "")
        {
            return (string.IsNullOrWhiteSpace(type) && productVariant.DownloadFileUrl != null)
                   || (!string.IsNullOrWhiteSpace(type) && type == "demo" && productVariant.DemoFileUrl != null)
                       ? null
                       : new DownloadProductVariantResult(
                             "Download link not found. Please contact the site owner for assistance.");
        }
    }
}