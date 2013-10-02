using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Users;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class AddressController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;

        public AddressController(IOrderService orderService, ICountryService countryService, IAddressService addressService)
        {
            _orderService = orderService;
            _countryService = countryService;
            _addressService = addressService;
        }

        [HttpGet]
        public ActionResult Edit(Order order)
        {
            ViewData["Countries"] = _countryService.GetOptions();
            return PartialView(order);
            
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Order order)
        {
            
            _orderService.Save(order);
            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }
    }
}