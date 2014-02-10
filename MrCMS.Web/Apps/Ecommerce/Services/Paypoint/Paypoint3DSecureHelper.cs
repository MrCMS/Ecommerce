using System;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public class Paypoint3DSecureHelper : IPaypoint3DSecureHelper
    {
        private const string CurrentPaypointOrderAmount = "current.paypoint.order-amount";
        private readonly ICartGuidResetter _cartGuidResetter;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private const string CurrentPaypointOrderGuid = "current.paypoint.orderguid";

        public Paypoint3DSecureHelper(ICartGuidResetter cartGuidResetter, ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _cartGuidResetter = cartGuidResetter;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public Guid ResetCartGuid()
        {
            return _cartGuidResetter.ResetCartGuid(_getUserGuid.UserGuid);
        }

        public decimal GetOrderAmount()
        {
            return _cartSessionManager.GetSessionValue<decimal>(CurrentPaypointOrderAmount, _getUserGuid.UserGuid);
        }

        public void SetOrderAmount(decimal total)
        {
            _cartSessionManager.SetSessionValue(CurrentPaypointOrderAmount, _getUserGuid.UserGuid, total);
        }

        public Guid GetCartGuid()
        {
            return _cartSessionManager.GetSessionValue(CurrentPaypointOrderGuid, _getUserGuid.UserGuid, Guid.Empty);
        }

        public void SetCartGuid(Guid cartGuid)
        {
            _cartSessionManager.SetSessionValue(CurrentPaypointOrderGuid, _getUserGuid.UserGuid, cartGuid);
        }
    }
}