using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartBuilder
    {
        CartModel BuildCart();
        CartModel BuildCart(Guid userGuid);
    }

    public interface ICartGuidResetter
    {
        Guid ResetCartGuid(Guid userGuid);
    }

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