using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public interface IDownloadOrderedFileService
    {
        FilePathResult GetDownload(Order order, OrderLine orderLine);
    }
}