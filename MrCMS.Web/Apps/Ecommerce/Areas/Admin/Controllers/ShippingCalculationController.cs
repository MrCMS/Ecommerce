using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingCalculationManager _shippingCalculationManager;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly ICountryService _countryService;

        public ShippingCalculationController(IShippingCalculationManager shippingCalculationManager, IShippingMethodManager shippingMethodManager, ICountryService countryService)
        {
            _shippingCalculationManager = shippingCalculationManager;
            _shippingMethodManager = shippingMethodManager;
            _countryService = countryService;
        }

        public ViewResult Index()
        {
            return View(_countryService.GetAllCountries());
        }

        [HttpGet]
        public ActionResult Add(Country country)
        {
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();
            ViewData["shipping-methods"] = _shippingMethodManager.GetOptions();
            var shippingCalculation = new ShippingCalculation { Country=country};
            return PartialView(shippingCalculation);
        }

        [ActionName("Add")]
        [HttpPost]
        public ActionResult Add_POST(ShippingCalculation shippingCalculation)
        {
            if (ModelState.IsValid)
            {
                if (shippingCalculation.ShippingMethod != null)
                {
                    _shippingCalculationManager.Add(shippingCalculation);
                }
                return RedirectToAction("Index");
            }
            return PartialView(shippingCalculation);
        }

        [HttpGet]
        public PartialViewResult Edit(ShippingCalculation shippingCalculation)
        {
            ViewData["shipping-methods"] = _shippingMethodManager.GetOptions();
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();

            return PartialView(shippingCalculation);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(ShippingCalculation shippingCalculation)
        {
            if (ModelState.IsValid)
            {
                if (shippingCalculation.ShippingMethod != null)
                {
                    _shippingCalculationManager.Update(shippingCalculation);
                }
                return RedirectToAction("Index");
            }
            return PartialView(shippingCalculation);
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