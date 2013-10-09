using System;
using System.IO;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class GoogleBaseFeedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGoogleBaseManager _googleBaseManager;

        public GoogleBaseFeedController(IGoogleBaseManager googleBaseManager)
        {
            _googleBaseManager = googleBaseManager;
        }

        public ActionResult ExportProductsToGoogleBaseInResponse()
        {
            try
            {
                var file = _googleBaseManager.ExportProductsToGoogleBase();
                return new FileStreamResult(new MemoryStream(file), "text/xml");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                const string msg = "Google Base exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("Dashboard","GoogleBase", new { status = msg });
            }
        }
    }
}