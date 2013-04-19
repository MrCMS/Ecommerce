using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingCalculationManager
    {
        List<SelectListItem> GetCriteriaOptions();
        void Add(ShippingCalculation shippingCalculation);
        void Update(ShippingCalculation shippingCalculation);
        void Delete(ShippingCalculation shippingCalculation);
    }
}