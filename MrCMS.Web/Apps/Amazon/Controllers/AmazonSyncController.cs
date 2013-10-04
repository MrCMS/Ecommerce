using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Amazon.Controllers
{
    public class AmazonSyncController : MrCMSAppUIController<AmazonApp>
    {
        private readonly IAmazonOrderSyncService _amazonOrderSyncService;

        public AmazonSyncController(IAmazonOrderSyncService amazonOrderSyncService)
        {
            _amazonOrderSyncService = amazonOrderSyncService;
        }

        public ActionResult Sync()
        {
            _amazonOrderSyncService.Sync();
            return new EmptyResult();
        } 
    }
}