using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BulkShippingController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBulkShippingService _bulkShippingService;

        public BulkShippingController(IBulkShippingService bulkShippingService)
        {
            _bulkShippingService = bulkShippingService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(BulkShippingUpdateACL), BulkShippingUpdateACL.Update)]
        public ViewResult Update()
        {
            if (TempData.ContainsKey("messages"))
                ViewBag.Messages = TempData["messages"];
            if (TempData.ContainsKey("import-status"))
                ViewBag.ImportStatus = TempData["import-status"];
            return View();
        }

        [HttpPost]
        [ActionName("Update")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(BulkShippingUpdateACL), BulkShippingUpdateACL.Update)]
        public RedirectToRouteResult Update_POST(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 &&
                (document.ContentType.ToLower() == "text/csv" || document.ContentType.ToLower().Contains("excel")))
                TempData["messages"] = _bulkShippingService.Update(document.InputStream);
            else
                TempData["import-status"] = "Please choose non-empty CSV (.csv) file before uploading.";
            return RedirectToAction("Update");
        }
    }
}