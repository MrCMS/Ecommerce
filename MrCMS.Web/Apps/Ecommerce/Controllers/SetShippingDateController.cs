using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetShippingDateController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISetShippingDateService _setShippingDateService;

        public SetShippingDateController(ISetShippingDateService setShippingDateService)
        {
            _setShippingDateService = setShippingDateService;
        }

        [HttpPost]
        public ActionResult SetDate(DateTime date)
        {
            _setShippingDateService.SetDate(date);
            return Request.IsAjaxRequest()
                ? Json(true)
                : _setShippingDateService.RedirectToSetShippingDetails();
        }
    }
}