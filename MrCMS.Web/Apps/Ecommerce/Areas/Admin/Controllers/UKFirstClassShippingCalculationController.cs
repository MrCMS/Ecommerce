using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UKFirstClassShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUKFirstClassShippingCalculationAdminService _adminService;

        public UKFirstClassShippingCalculationController(IUKFirstClassShippingCalculationAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewData["criteria-options"] = _adminService.GetCriteriaOptions();
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(UKFirstClassShippingCalculation calculation)
        {
            _adminService.Add(calculation);
            TempData.SuccessMessages().Add("Calculation added successfully");
            return RedirectToAction("Configure", "UKFirstClassShipping");
        }

        [HttpGet]
        public PartialViewResult Edit(UKFirstClassShippingCalculation calculation)
        {
            ViewData["criteria-options"] = _adminService.GetCriteriaOptions();
            return PartialView(calculation);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(UKFirstClassShippingCalculation calculation)
        {
            _adminService.Update(calculation);
            TempData.SuccessMessages().Add("Calculation updated successfully");
            return RedirectToAction("Configure", "UKFirstClassShipping");
        }

        [HttpGet]
        public PartialViewResult Delete(UKFirstClassShippingCalculation calculation)
        {
            return PartialView(calculation);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(UKFirstClassShippingCalculation calculation)
        {
            _adminService.Delete(calculation);
            TempData.SuccessMessages().Add("Calculation removed successfully");
            return RedirectToAction("Configure", "UKFirstClassShipping");
        }

        public JsonResult IsValidShippingCalculation(CalculationInfo calculationInfo)
        {
            return Json(_adminService.IsCalculationValid(calculationInfo), JsonRequestBehavior.AllowGet);
        }
    }
}