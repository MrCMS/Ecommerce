using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignBasicCartInfo : IAssignBasicCartInfo
    {
        private readonly ICartGuidResetter _cartGuidResetter;
        private readonly IGetBillingAddressSameAsShippingAddress _billingAddressSameAsShippingAddress;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignBasicCartInfo(ISession session, ICartSessionManager cartSessionManager,
            ICartGuidResetter cartGuidResetter, IGetBillingAddressSameAsShippingAddress billingAddressSameAsShippingAddress)
        {
            _session = session;
            _cartSessionManager = cartSessionManager;
            _cartGuidResetter = cartGuidResetter;
            _billingAddressSameAsShippingAddress = billingAddressSameAsShippingAddress;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            List<CartItem> cartItems = GetItems(userGuid);
            DeleteNullProducts(cartItems);
            cart.CartGuid = GetCartGuid(userGuid);
            cart.User = CurrentRequestData.CurrentUser;
            cart.UserGuid = userGuid;
            cart.Items = cartItems;
            cart.OrderEmail = GetOrderEmail(userGuid);
            cart.GiftMessage = GetGiftMessage(userGuid);
            cart.BillingAddressSameAsShippingAddress = _billingAddressSameAsShippingAddress.Get(cart, userGuid);
            return cart;
        }

        private string GetGiftMessage(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentGiftMessageKey, userGuid);
        }


        private string GetOrderEmail(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartManager.CurrentOrderEmailKey, userGuid);
        }

        private List<CartItem> GetItems(Guid userGuid)
        {
            return
                _session.QueryOver<CartItem>()
                    .Where(item => item.UserGuid == userGuid)
                    .Cacheable()
                    .List().ToList();
        }

        private Guid GetCartGuid(Guid userGuid)
        {
            Guid value = _cartSessionManager.GetSessionValue(CartManager.CurrentCartGuid, userGuid, Guid.Empty);
            if (value == Guid.Empty)
            {
                value = _cartGuidResetter.ResetCartGuid(userGuid);
            }
            return value;
        }


        private void DeleteNullProducts(IEnumerable<CartItem> items)
        {
            foreach (CartItem cartItem in items.Where(x => x.Item == null || x.Item.Product == null))
            {
                CartItem item = cartItem;
                _session.Transact(session => _session.Delete(item));
            }
        }
    }

    public interface IGetBillingAddressSameAsShippingAddress
    {
        bool Get(CartModel cart, Guid userGuid);
    }

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