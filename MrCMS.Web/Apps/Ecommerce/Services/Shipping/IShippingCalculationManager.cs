using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingCalculationManager
    {
        IList<ShippingCalculation> GetAll();
        ShippingCalculation Get(int id);
        List<SelectListItem> GetOptions();
        List<SelectListItem> GetCriteriaOptions();
        ShippingCalculation GetByCountryId(int countryId);
        void Add(ShippingCalculation shippingCalculation);
        void Update(ShippingCalculation shippingCalculation);
        void Delete(ShippingCalculation shippingCalculation);
    }
}