using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetShippingMethodController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISetShippingMethodService _setShippingMethodService;
        private readonly CartModel _cartModel;

        public SetShippingMethodController(ISetShippingMethodService setShippingMethodService, CartModel cartModel)
        {
            _setShippingMethodService = setShippingMethodService;
            _cartModel = cartModel;
        }

        public PartialViewResult AwaitingAddress()
        {
            return PartialView();
        }

        public PartialViewResult ShippingOptions()
        {
            ViewData["shipping-methods"] = _setShippingMethodService.GetShippingMethodOptions();
            return PartialView(_cartModel);
        }

        public ActionResult SetShipping(
            [IoCModelBinder(typeof (ShippingMethodModelBinder))] IShippingMethod shippingMethod)
        {
            _setShippingMethodService.SetShippingMethod(shippingMethod);
            if (Request.IsAjaxRequest())
                return Json(true);
            return _setShippingMethodService.RedirectToShippingDetails();
        }
    }
}