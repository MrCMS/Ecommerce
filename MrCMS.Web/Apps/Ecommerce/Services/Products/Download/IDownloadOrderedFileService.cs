using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public interface IDownloadOrderedFileService
    {
        ActionResult WriteDownloadToResponse(HttpResponseBase response, Order order, OrderLine orderLine);
    }
}