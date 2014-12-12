using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce;
using MrCMS.Web.Areas.Admin.Services.NopImport;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NopDataImportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly INopDataImportAdminService _nopDataImportAdminService;

        public NopDataImportController(INopDataImportAdminService nopDataImportAdminService)
        {
            _nopDataImportAdminService = nopDataImportAdminService;
        }

        public ViewResult Index()
        {
            ViewData["importer-options"] = _nopDataImportAdminService.GetImporterOptions();
            ViewData["result"] = TempData["result"];
            return View(new ImportParams());
        }

        [HttpPost]
        public RedirectToRouteResult Import(ImportParams importParams)
        {
            TempData["result"] = _nopDataImportAdminService.Import(importParams);
            return RedirectToAction("Index");
        }
    }
}