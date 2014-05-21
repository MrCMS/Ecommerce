using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BrandController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBrandService _brandService;
        private readonly SiteSettings _siteSettings;

        public BrandController(IBrandService brandService, SiteSettings siteSettings)
        {
            _brandService = brandService;
            _siteSettings = siteSettings;
        }

        [HttpGet]
        public ViewResult Index(string q, int page = 1)
        {
            ViewData["query"] = q;
            var brands = _brandService.GetPaged(page, q, _siteSettings.DefaultPageSize);
            return View(brands);
        }

        [HttpGet]
        public PartialViewResult Add(int productId=0)
        {
            ViewBag.ProductID = productId;
            return PartialView(new Brand());
        }

        [ActionName("Add")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Add_POST(Brand brand, int productId=0)
        {
            if (ModelState.IsValid)
            {
                _brandService.Add(brand);
                return productId==0 ? RedirectToAction("Index") : RedirectToAction("Edit", "Webpage", new { id = productId });
            }
            return PartialView(brand);
        }

        [HttpGet]
        public ViewResult Edit(Brand brand)
        {
            return View(brand);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit_POST(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _brandService.Update(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [HttpGet]
        public PartialViewResult Delete(Brand brand)
        {
            return PartialView(brand);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(Brand brand)
        {
            _brandService.Delete(brand);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult IsUniqueName(string name, int id=0)
        {
            return _brandService.AnyExistingBrandsWithName(name, id) ? 
                Json("There is already a brand stored with that name.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}