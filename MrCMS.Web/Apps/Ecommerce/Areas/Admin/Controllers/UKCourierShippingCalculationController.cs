using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class UKCourierShippingCalculationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IUKCourierShippingCalculationAdminService _adminService;

        public UKCourierShippingCalculationController(IUKCourierShippingCalculationAdminService adminService)
        {
            _adminService = adminService;
        }

        public PartialViewResult Add()
        {
            ViewData["criteria-options"] = _adminService.GetCriteriaOptions();
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(UKCourierShippingCalculation calculation)
        {
            _adminService.Add(calculation);
            TempData.SuccessMessages().Add("Calculation added successfully");
            return RedirectToAction("Configure", "UKCourierShipping");
        }

        [HttpGet]
        public PartialViewResult Edit(UKCourierShippingCalculation calculation)
        {
            ViewData["criteria-options"] = _adminService.GetCriteriaOptions();
            return PartialView(calculation);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(UKCourierShippingCalculation calculation)
        {
            _adminService.Update(calculation);
            TempData.SuccessMessages().Add("Calculation updated successfully");
            return RedirectToAction("Configure", "UKCourierShipping");
        }

        [HttpGet]
        public PartialViewResult Delete(UKCourierShippingCalculation calculation)
        {
            return PartialView(calculation);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(UKCourierShippingCalculation calculation)
        {
            _adminService.Delete(calculation);
            TempData.SuccessMessages().Add("Calculation removed successfully");
            return RedirectToAction("Configure", "UKCourierShipping");
        }
    }
}