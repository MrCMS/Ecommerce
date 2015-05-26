using System;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetShippingAddress : IGetShippingAddress
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetCurrentUser _getCurrentUser;

        public GetShippingAddress(ICartSessionManager cartSessionManager,IGetCurrentUser getCurrentUser)
        {
            _cartSessionManager = cartSessionManager;
            _getCurrentUser = getCurrentUser;
        }

        public Address Get(Guid userGuid)
        {
            var shippingAddress = _cartSessionManager.GetSessionValue<Address>(CartManager.CurrentShippingAddressKey, userGuid);
            if (shippingAddress != null)
            {
                shippingAddress.User = _getCurrentUser.Get();
            }
            return shippingAddress;
        }
    }
}