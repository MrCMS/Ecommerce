using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BrandController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBrandAdminService _brandAdminService;

        public BrandController(IBrandAdminService brandAdminService)
        {
            _brandAdminService = brandAdminService;
        }

        public ViewResult Index(BrandSearchModel searchModel)
        {
            ViewData["listing-page"] = _brandAdminService.GetListingPage();
            ViewData["results"] = _brandAdminService.Search(searchModel);
            ViewData["any-to-migrate"] = _brandAdminService.AnyToMigrate();
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult Migrate()
        {
            return View();
        }

        [HttpPost, ActionName("Migrate")]
        public ActionResult Migrate_POST()
        {
            _brandAdminService.MigrateBrands();
            TempData.SuccessMessages().Add("Brands migrated to pages.");
            return RedirectToAction("Index");
        }

        public PartialViewResult ForProduct(Product product)
        {
            ViewData["brands"] = _brandAdminService.GetOptions();
            return PartialView(product);
        }
    }
}