using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class DownloadOrderedFileController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IDownloadOrderedFileService _downloadOrderedFileService;

        public DownloadOrderedFileController(IDownloadOrderedFileService downloadOrderedFileService)
        {
            _downloadOrderedFileService = downloadOrderedFileService;
        }

        public ActionResult Download([IoCModelBinder(typeof(DownloadOrderedFileOrderByGuidModelBinder))] Order order, OrderLine line)
        {
            _downloadOrderedFileService.WriteDownloadToResponse(Response, order, line);
            return new EmptyResult();
        }
    }
}
