using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetBillingAddressSameAsShippingAddress : IGetBillingAddressSameAsShippingAddress
    {
        private readonly ICartSessionManager _cartSessionManager;

        public GetBillingAddressSameAsShippingAddress(ICartSessionManager cartSessionManager)
        {
            _cartSessionManager = cartSessionManager;
        }

        public bool Get(CartModel cart, Guid userGuid)
        {
            return cart.RequiresShipping && GetSessionValue(userGuid);
        }
        private bool GetSessionValue(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue(CartManager.CurrentBillingAddressSameAsShippingAddressKey,
                userGuid, true);
        }
    }
}