using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Website;
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
            var errorMessage = string.Empty;
            try
            {
                _amazonOrderSyncService.Sync();
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                errorMessage += Environment.NewLine;
                errorMessage += ex.StackTrace;
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return Content(errorMessage, "text/plain");
        } 
    }
}