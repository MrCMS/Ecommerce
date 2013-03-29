using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IShippingCalculationManager
    {
        IList<ShippingCalculation> GetAll();
        ShippingCalculation Get(int id);
        List<SelectListItem> GetOptions();
        List<SelectListItem> GetCriteriaOptions();
        ShippingCalculation GetByCountryId(int countryId);
        void Add(ShippingCalculation ShippingCalculation);
        void Update(ShippingCalculation ShippingCalculation);
        void Delete(ShippingCalculation ShippingCalculation);
    }
}