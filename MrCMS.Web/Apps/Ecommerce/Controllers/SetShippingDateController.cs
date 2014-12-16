using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetShippingDateController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISetShippingMethodDateService _setShippingMethodDateService;

        public SetShippingDateController(ISetShippingMethodDateService setShippingMethodDateService)
        {
            _setShippingMethodDateService = setShippingMethodDateService;
        }

        [HttpPost]
        public ActionResult SetDate(DateTime date)
        {
            _setShippingMethodDateService.SetDate(date);
            return Request.IsAjaxRequest()
                ? Json(true)
                : _setShippingMethodDateService.RedirectToSetShippingDetails();
        }
    }
}