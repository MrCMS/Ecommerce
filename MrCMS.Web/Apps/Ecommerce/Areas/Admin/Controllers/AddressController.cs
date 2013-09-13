using System.Web.Mvc;
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
        public ActionResult Edit(int addressType=0, int orderID = 0, int addressID = 0)
        {
            ViewData["Countries"] = _countryService.GetOptions();
            ViewData["addressID"] = addressID;
            ViewData["addressType"] = addressType;
            if (addressID != 0)
            {
                var address = _addressService.Get(addressID);
                if(address.Country!=null)
                    ViewData["countryID"] = address.Country.Id;
                return PartialView(address);
            }

            return PartialView(new Address());
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(Address item, int orderID, int addressType, int countryID=0)
        {
            if (countryID != 0)
                item.Country = _countryService.Get(countryID);
            _addressService.Save(item);
            var order=_orderService.Get(orderID);
            if (addressType == 1)
                order.ShippingAddress = item;
            else
                order.BillingAddress = item;
            _orderService.Save(order);
            return RedirectToAction("Edit", "Order", new { id = orderID });
        }
    }
}