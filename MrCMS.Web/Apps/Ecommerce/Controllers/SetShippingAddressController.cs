using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetShippingAddressController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISetShippingAddressService _setShippingAddressService;

        public SetShippingAddressController(ISetShippingAddressService setShippingAddressService)
        {
            _setShippingAddressService = setShippingAddressService;
        }

        public PartialViewResult ShippingAddress()
        {
            ViewData["other-addresses"] = _setShippingAddressService.GetOtherAddresses();
            ViewData["country-options"] = _setShippingAddressService.GetCountryOptions();
            return PartialView(_setShippingAddressService.GetShippingAddress());
        }

        public PartialViewResult ShowShippingAddress()
        {
            return PartialView(_setShippingAddressService.GetShippingAddress());
        }
        

        [HttpPost]
        public ActionResult SetAddress(Address address)
        {
            _setShippingAddressService.SetShippingAddress(address);
            if (Request.IsAjaxRequest())
                return Json(true);
            return _setShippingAddressService.RedirectToDetails();
        }
    }
}