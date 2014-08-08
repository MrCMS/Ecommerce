using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignShippingInfo : IAssignShippingInfo
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetShippingAddress _getShippingAddress;
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public AssignShippingInfo(ICartSessionManager cartSessionManager, IGetShippingAddress getShippingAddress, IShippingMethodUIService shippingMethodUIService)
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
                cart.AvailableShippingMethods =
                    _shippingMethodUIService.GetAvailableMethods().FindAll(method => cart.ShippingAddress == null || method.CanBeUsed(cart));
                cart.ShippingMethod = GetShippingMethod(cart, userGuid);
            }
            return cart;
        }

        private IShippingMethod GetShippingMethod(CartModel cart, Guid userGuid)
        {
            var type = _cartSessionManager.GetSessionValue<string>(CartManager.CurrentShippingMethodTypeKey, userGuid);
            return cart.AvailableShippingMethods.FirstOrDefault(method => method.GetType().FullName == type);
        }
    }
}