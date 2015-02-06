using System;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignShippingInfo : IAssignShippingInfo
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetShippingAddress _getShippingAddress;
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public AssignShippingInfo(ICartSessionManager cartSessionManager, IGetShippingAddress getShippingAddress,
            IShippingMethodUIService shippingMethodUIService)
        {
            _cartSessionManager = cartSessionManager;
            _getShippingAddress = getShippingAddress;
            _shippingMethodUIService = shippingMethodUIService;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.ShippingAddress = _getShippingAddress.Get(userGuid);
            if (cart.RequiresShipping)
            {
                cart.PotentiallyAvailableShippingMethods =
                    _shippingMethodUIService.GetEnabledMethods().FindAll(method => method.CanPotentiallyBeUsed(cart));
                cart.RequestedShippingDate =
                    _cartSessionManager.GetSessionValue<DateTime?>(CartManager.CurrentShippingDateKey, userGuid);
                cart.ShippingMethod = GetShippingMethod(cart, userGuid);
            }
            return cart;
        }

        private IShippingMethod GetShippingMethod(CartModel cart, Guid userGuid)
        {
            var type = _cartSessionManager.GetSessionValue<string>(CartManager.CurrentShippingMethodTypeKey, userGuid);
            IShippingMethod shippingMethod = _shippingMethodUIService.GetMethodByTypeName(type);
            if (shippingMethod != null)
                return shippingMethod.CanBeUsed(cart) ? shippingMethod : null;
            return null;
        }
    }
}