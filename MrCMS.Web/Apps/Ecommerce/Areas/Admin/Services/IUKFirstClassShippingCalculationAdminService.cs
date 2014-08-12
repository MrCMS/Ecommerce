using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUKFirstClassShippingCalculationAdminService
    {
        List<SelectListItem> GetCriteriaOptions();
        bool IsCalculationValid(CalculationInfo calculationInfo);
        void Add(UKFirstClassShippingCalculation calculation);
        void Update(UKFirstClassShippingCalculation calculation);
        void Delete(UKFirstClassShippingCalculation calculation);
    }
}