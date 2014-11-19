using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Areas.Admin.Services.NopImport;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NopProductImportController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly INopProductImportAdminService _nopProductImportAdminService;

        public NopProductImportController(INopProductImportAdminService nopProductImportAdminService)
        {
            _nopProductImportAdminService = nopProductImportAdminService;
        }

        public ViewResult Index()
        {
            ViewData["importer-options"] = _nopProductImportAdminService.GetImporterOptions();
            return View();
        }
    }
}