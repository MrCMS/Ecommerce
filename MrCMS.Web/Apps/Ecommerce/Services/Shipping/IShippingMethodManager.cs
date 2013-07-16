using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingMethodManager
    {
        IList<ShippingMethod> GetAll();
        ShippingMethod Get(int id);
        List<SelectListItem> GetOptions();
        void Add(ShippingMethod ShippingMethod);
        void Update(ShippingMethod ShippingMethod);
        void Delete(ShippingMethod ShippingMethod);
    }
}
