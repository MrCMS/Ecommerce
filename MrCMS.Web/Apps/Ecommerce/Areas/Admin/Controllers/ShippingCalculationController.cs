using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingCalculationManager _shippingCalculationManager;
        private readonly IShippingMethodManager _shippingMethodManager;

        public ShippingCalculationController(IShippingCalculationManager shippingCalculationManager, IShippingMethodManager shippingMethodManager)
        {
            _shippingCalculationManager = shippingCalculationManager;
            _shippingMethodManager = shippingMethodManager;
        }

        public ViewResult Index()
        {
            return View(_shippingMethodManager.GetAll());
        }

        [HttpGet]
        public ActionResult Add(ShippingMethod method)
        {
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();
            var shippingCalculation = new ShippingCalculation { ShippingMethod = method };
            return PartialView(shippingCalculation);
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(ShippingCalculation shippingCalculation)
        {
            if (shippingCalculation.ShippingMethod != null)
            {
                _shippingCalculationManager.Add(shippingCalculation);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ShippingCalculation shippingCalculation)
        {
            ViewData["shippingmethods"] = _shippingMethodManager.GetOptions();
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();

            return PartialView(shippingCalculation);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(ShippingCalculation shippingCalculation)
        {
            if (shippingCalculation.ShippingMethod != null)
            {
                _shippingCalculationManager.Update(shippingCalculation);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ShippingCalculation shippingCalculation)
        {
            return PartialView(shippingCalculation);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(ShippingCalculation shippingCalculation)
        {
            _shippingCalculationManager.Delete(shippingCalculation);
            return RedirectToAction("Index");
        }

    }
}