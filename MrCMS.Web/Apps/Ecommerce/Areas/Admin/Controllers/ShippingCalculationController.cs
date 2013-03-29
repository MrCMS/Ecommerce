using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingCalculationManager _shippingCalculationManager;
        private readonly ICountryService _countryService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public ShippingCalculationController(IShippingCalculationManager shippingCalculationManager, ICountryService countryService, IShippingMethodManager shippingMethodManager)
        {
            _shippingCalculationManager = shippingCalculationManager;
            _countryService = countryService;
            _shippingMethodManager = shippingMethodManager;
        }

        public ViewResult Index()
        {
            return View(_countryService.GetAllCountries());
        }

        [HttpGet]
        public ActionResult Add(int? countryId)
        {
            if (countryId.HasValue)
            {
                ViewData["shippingmethods"] = _shippingMethodManager.GetOptions();
                ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();
                ShippingCalculation tr = new ShippingCalculation { Country = _countryService.Get(countryId.Value) };
                return PartialView(tr);
            }
            return RedirectToAction("Index");
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(ShippingCalculation ShippingCalculation)
        {
            if (ShippingCalculation.Country != null)
            {
                _shippingCalculationManager.Add(ShippingCalculation);
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
        public RedirectToRouteResult Edit_POST(ShippingCalculation ShippingCalculation)
        {
            if (ShippingCalculation.Country != null)
            {
                _shippingCalculationManager.Update(ShippingCalculation);
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