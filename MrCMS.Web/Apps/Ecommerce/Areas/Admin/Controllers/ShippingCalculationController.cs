using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Website.Filters;

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
        [ForceImmediateLuceneUpdate]
        public ActionResult Add_POST(ShippingCalculation shippingCalculation)
        {
            if (ModelState.IsValid)
            {
                if (shippingCalculation.ShippingMethod != null && _shippingCalculationManager.IsValidForAdding(shippingCalculation))
                {
                    _shippingCalculationManager.Add(shippingCalculation);
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "Shipping calculation with provided values overlap existing calculations in the system.";
            }
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();
            ViewData["shipping-methods"] = _shippingMethodManager.GetOptions();
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
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit_POST(ShippingCalculation shippingCalculation)
        {
            if (ModelState.IsValid)
            {
                if (shippingCalculation.ShippingMethod != null &&
                    _shippingCalculationManager.IsValidForAdding(shippingCalculation))
                {
                    _shippingCalculationManager.Update(shippingCalculation);
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "Shipping calculation with provided values overlap existing calculations in the system.";
            }
            ViewData["criterias"] = _shippingCalculationManager.GetCriteriaOptions();
            ViewData["shipping-methods"] = _shippingMethodManager.GetOptions();
            return PartialView(shippingCalculation);
        }

        [HttpGet]
        public PartialViewResult Delete(ShippingCalculation shippingCalculation)
        {
            return PartialView(shippingCalculation);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(ShippingCalculation shippingCalculation)
        {
            _shippingCalculationManager.Delete(shippingCalculation);
            return RedirectToAction("Index");
        }


        public JsonResult IsValidShippingCalculation(string shippingCriteria, ShippingMethod shippingMethod, Country country, decimal lowerBound = 0, decimal upperBound = 0, int id = 0)
        {
            var shippingCalculation = new ShippingCalculation
                {
                    LowerBound = lowerBound,
                    UpperBound = upperBound,
                    Id = id,
                    ShippingMethod = shippingMethod,
                    Country = country
                };

            if (!string.IsNullOrWhiteSpace(shippingCriteria))
            {
                shippingCalculation.ShippingCriteria =Enum.GetValues(typeof (ShippingCriteria)).Cast<ShippingCriteria>().SingleOrDefault(x => x.ToString() == shippingCriteria);
            }

            return !_shippingCalculationManager.IsValidForAdding(shippingCalculation)
                           ? Json("Shipping calculation with provided values overlap existing calculations in the system.", JsonRequestBehavior.AllowGet)
                           : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}