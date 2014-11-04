using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class SetShippingAddressService : ISetShippingAddressService
    {
        private readonly CartModel _cart;
        private readonly ICartManager _cartManager;
        private readonly IGetCountryOptions _getCountryOptions;
        private readonly IGetExistingAddressOptions _getExistingAddressOptions;
        private readonly IUniquePageService _uniquePageService;

        public SetShippingAddressService(IUniquePageService uniquePageService, ICartManager cartManager, CartModel cart,
            IGetCountryOptions getCountryOptions,
            IGetExistingAddressOptions getExistingAddressOptions)
        {
            _uniquePageService = uniquePageService;
            _cartManager = cartManager;
            _cart = cart;
            _getCountryOptions = getCountryOptions;
            _getExistingAddressOptions = getExistingAddressOptions;
        }

        public void SetShippingAddress(Address address)
        {
            _cartManager.SetShippingAddress(address);
        }

        public Address GetShippingAddress()
        {
            return _cart.ShippingAddress;
        }

        public List<SelectListItem> GetCountryOptions()
        {
            return _getCountryOptions.Get();
        }

        public List<SelectListItem> GetOtherAddresses()
        {
            Address shippingAddress = _cart.ShippingAddress ?? new Address();
            return _getExistingAddressOptions.Get(shippingAddress);
        }

        public ActionResult RedirectToDetails()
        {
            return _uniquePageService.RedirectTo<SetShippingDetails>();
        }
    }
}