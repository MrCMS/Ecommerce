using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetShippingAddress : IGetShippingAddress
    {
        private readonly ICartSessionManager _cartSessionManager;

        public GetShippingAddress(ICartSessionManager cartSessionManager)
        {
            _cartSessionManager = cartSessionManager;
        }

        public Address Get(Guid userGuid)
        {
            var shippingAddress = _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentShippingAddressKey, userGuid);
            if (shippingAddress != null)
            {
                shippingAddress.User = CurrentRequestData.CurrentUser;
            }
            return shippingAddress;
        }
    }
}