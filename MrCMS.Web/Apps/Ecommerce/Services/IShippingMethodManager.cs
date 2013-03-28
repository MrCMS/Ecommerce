using MrCMS.Web.Apps.Ecommerce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IShippingMethodManager
    {
        IList<ShippingMethod> GetAll();
        List<SelectListItem> GetOptions();
        void Add(ShippingMethod ShippingMethod);
        void Update(ShippingMethod ShippingMethod);
        void Delete(ShippingMethod ShippingMethod);
    }
}
