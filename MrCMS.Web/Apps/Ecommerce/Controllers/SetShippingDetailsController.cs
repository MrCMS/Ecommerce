using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetShippingDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISetShippingDetailsUIService _setShippingDetailsUIService;

        public SetShippingDetailsController(ISetShippingDetailsUIService setShippingDetailsUIService)
        {
            _setShippingDetailsUIService = setShippingDetailsUIService;
        }

        [CanEnterCheckout]
        public ActionResult Show(SetShippingDetails page)
        {
            if (_setShippingDetailsUIService.UserRequiresRedirect())
                return _setShippingDetailsUIService.UserRedirect();
            ViewData["cart"] = _setShippingDetailsUIService.Cart;
            return View(page);
        }
    }
}