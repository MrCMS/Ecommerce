using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BulkStockUpdateController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBulkStockUpdateAdminService _bulkStockUpdateAdminService;

        public BulkStockUpdateController(IBulkStockUpdateAdminService bulkStockUpdateAdminService)
        {
            _bulkStockUpdateAdminService = bulkStockUpdateAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof (BulkStockUpdateACL), BulkStockUpdateACL.BulkStockUpdate)]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof (BulkStockUpdateACL), BulkStockUpdateACL.BulkStockUpdate)]
        public RedirectToRouteResult Index_POST(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 &&
                (document.ContentType.ToLower() == "text/csv" || document.ContentType.ToLower().Contains("excel")))
            {
                var result = _bulkStockUpdateAdminService.BulkStockUpdate(document.InputStream);
                if (result.IsSuccess)
                    TempData.SuccessMessages().AddRange(result.Messages);
                else
                    TempData.ErrorMessages().AddRange(result.Messages);
            }
            else
                TempData.ErrorMessages().Add("Please choose non-empty CSV (.csv) file before uploading.");
            return RedirectToAction("Index");
        }
    }
}