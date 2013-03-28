using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingMethodController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingMethodManager _ShippingMethodManager;

        public ShippingMethodController(IShippingMethodManager ShippingMethodManager)
        {
            _ShippingMethodManager = ShippingMethodManager;
        }

        public ViewResult Index()
        {
            var options = _ShippingMethodManager.GetAll();
            return View(options);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(ShippingMethod option)
        {
            _ShippingMethodManager.Add(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ShippingMethod option)
        {
            return PartialView(option);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(ShippingMethod option)
        {
            _ShippingMethodManager.Update(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ShippingMethod option)
        {
            return PartialView(option);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(ShippingMethod option)
        {
            _ShippingMethodManager.Delete(option);
            return RedirectToAction("Index");
        }
    }
}