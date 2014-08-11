using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUKCourierShippingCalculationAdminService
    {
        void Add(UKCourierShippingCalculation calculation);
        void Update(UKCourierShippingCalculation calculation);
        void Delete(UKCourierShippingCalculation calculation);

        bool IsCalculationValid(CalculationInfo calculationInfo);
        List<SelectListItem> GetCriteriaOptions();
    }
}