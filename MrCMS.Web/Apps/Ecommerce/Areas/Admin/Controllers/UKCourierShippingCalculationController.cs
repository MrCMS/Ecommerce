using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Website.Controllers;
using NHibernate;

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

        public JsonResult IsValidShippingCalculation(CalculationInfo calculationInfo)
        {
            return Json(_adminService.IsCalculationValid(calculationInfo), JsonRequestBehavior.AllowGet);
        }
    }

    public interface IUKCourierShippingCalculationAdminService
    {
        List<SelectListItem> GetCriteriaOptions();
        void Add(UKCourierShippingCalculation calculation);
        bool IsCalculationValid(CalculationInfo calculationInfo);
    }

    public class UKCourierShippingCalculationAdminService : IUKCourierShippingCalculationAdminService
    {
        private readonly ISession _session;

        public UKCourierShippingCalculationAdminService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return Enum.GetValues(typeof (ShippingCriteria)).Cast<ShippingCriteria>()
                .BuildSelectItemList(criteria => criteria.GetDescription(),
                    criteria => criteria.ToString(), emptyItem: null);

        }

        public void Add(UKCourierShippingCalculation calculation)
        {
            _session.Transact(session => session.Save(calculation));
        }

        public bool IsCalculationValid(CalculationInfo calculationInfo)
        {
            var lowerBound = calculationInfo.LowerBound;
            var upperBound = calculationInfo.UpperBound.HasValue ? calculationInfo.UpperBound.Value : 0;
            var calcs =
                _session.QueryOver<UKCourierShippingCalculation>()
                    .Where(x => x.Id != calculationInfo.Id && x.ShippingCriteria == calculationInfo.ShippingCriteria)
                    .Cacheable().List();
            if (upperBound > 0)
                return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound)
                                       || (x.LowerBound <= upperBound && (upperBound <= x.UpperBound || x.UpperBound == null)));
            return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound) || x.UpperBound == null);
        }
    }
}