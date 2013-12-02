using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules
{

    public class OrderLineBelongsToOrder : IDownloadOrderedFileValidationRule
    {
        public IEnumerable<string> GetErrors(Order order, OrderLine orderLine)
        {
            if (!order.OrderLines.Contains(orderLine))
                yield return "Order line does not belong to order.";
        }
    }

    public class OrderLineIsDownloadable : IDownloadOrderedFileValidationRule
    {
        public IEnumerable<string> GetErrors(Order order, OrderLine orderLine)
        {
            if (!orderLine.IsDownloadable)
                yield return "Order line is not downloadable.";
        }
    }

    public class DownloadLinkIsNotTooOldForDownload : IDownloadOrderedFileValidationRule
    {
        public IEnumerable<string> GetErrors(Order order, OrderLine orderLine)
        {
            if (orderLine.DownloadExpiresOn > CurrentRequestData.Now)
                yield return "Download link has expired.";
        }
    }

    public class AllowedNumberOfDownloadsAvailable : IDownloadOrderedFileValidationRule
    {
        public IEnumerable<string> GetErrors(Order order, OrderLine orderLine)
        {
            if (orderLine.AllowedNumberOfDownloads.HasValue && orderLine.NumberOfDownloads >= orderLine.AllowedNumberOfDownloads)
                yield return "Download limit reached.";
        }
    }
}