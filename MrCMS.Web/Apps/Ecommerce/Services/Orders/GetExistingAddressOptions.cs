using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class GetExistingAddressOptions : IGetExistingAddressOptions
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly CartModel _cart;
        private readonly IBelongToUserLookupService _belongToUserLookupService;

        public GetExistingAddressOptions(IGetCurrentUser getCurrentUser, CartModel cart, IBelongToUserLookupService belongToUserLookupService)
        {
            _getCurrentUser = getCurrentUser;
            _cart = cart;
            _belongToUserLookupService = belongToUserLookupService;
        }

        public List<SelectListItem> Get(IAddress addressToExclude)
        {
            var addresses = new List<Address>();
            if (_cart.ShippingAddress != null) addresses.Add(_cart.ShippingAddress);
            if (_cart.BillingAddress != null) addresses.Add(_cart.BillingAddress);

            var currentUser = _getCurrentUser.Get();
            if (currentUser != null)
                addresses.AddRange(_belongToUserLookupService.GetAll<Address>(currentUser));

            var enumerable = addresses.Distinct(AddressComparison.Comparer).OfType<Address>();
            addresses = enumerable.Where(a => addressToExclude == null || !AddressComparison.Comparer.Equals(a, addressToExclude)).ToList();

            return addresses.Any()
                ? addresses.BuildSelectItemList(address => address.GetDescription(), address => address.ToJSON(),
                    emptyItemText: "Select an address...")
                : new List<SelectListItem>();
        }
    }
}