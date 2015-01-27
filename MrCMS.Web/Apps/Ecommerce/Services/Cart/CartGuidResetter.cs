using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartGuidResetter : ICartGuidResetter
    {
        private readonly ICartSessionManager _cartSessionManager;

        public CartGuidResetter(ICartSessionManager cartSessionManager)
        {
            _cartSessionManager = cartSessionManager;
        }

        public Guid ResetCartGuid(Guid userGuid)
        {
            var value = Guid.NewGuid();
            _cartSessionManager.SetSessionValue(CartManager.CurrentCartGuid, userGuid, value);
            return value;
        }
    }
}