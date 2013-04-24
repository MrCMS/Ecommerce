using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System;
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
        public PartialViewResult Add()
        {
            return PartialView(new Brand());
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(Brand brand)
        {
            _brandService.Add(brand);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Edit(Brand brand)
        {
            return View(brand);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Brand brand)
        {
            _brandService.Update(brand);
            return RedirectToAction("Index");
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
    }
}