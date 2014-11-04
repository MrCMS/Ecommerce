using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ISetShippingMethodService
    {
        List<SelectListItem> GetShippingMethodOptions();
        CartModel Cart { get; }
        void SetShippingMethod(IShippingMethod shippingMethod);
        ActionResult RedirectToShippingDetails();
    }
}