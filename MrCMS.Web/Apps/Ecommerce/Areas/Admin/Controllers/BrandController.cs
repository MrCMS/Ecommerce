using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class BrandController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public ViewResult Index(string q, int page = 1)
        {
            ViewData["query"] = q;
            var brands = _brandService.GetPaged(page, q);
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
        public ActionResult Add_POST(Brand brand, int productId=0)
        {
            if (ModelState.IsValid)
            {
                _brandService.Add(brand);
                if(productId==0)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Edit", "Webpage", new { id = productId });
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
        public RedirectToRouteResult Delete_POST(Brand brand)
        {
            _brandService.Delete(brand);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult IsUniqueName(string name, int id=0)
        {
            if (_brandService.AnyExistingBrandsWithName(name, id))
                return Json("There is already a brand stored with that name.", JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);

        }
    }
}