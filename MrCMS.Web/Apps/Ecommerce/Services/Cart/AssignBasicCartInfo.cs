using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignBasicCartInfo : IAssignBasicCartInfo
    {
        private readonly ICartGuidResetter _cartGuidResetter;
        private readonly IGetBillingAddressSameAsShippingAddress _billingAddressSameAsShippingAddress;
        private readonly ICartItemAvailablityService _cartItemAvailablityService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignBasicCartInfo(ISession session, ICartSessionManager cartSessionManager,
            ICartGuidResetter cartGuidResetter,
            IGetBillingAddressSameAsShippingAddress billingAddressSameAsShippingAddress,
            ICartItemAvailablityService cartItemAvailablityService, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _cartSessionManager = cartSessionManager;
            _cartGuidResetter = cartGuidResetter;
            _billingAddressSameAsShippingAddress = billingAddressSameAsShippingAddress;
            _cartItemAvailablityService = cartItemAvailablityService;
            _ecommerceSettings = ecommerceSettings;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            List<CartItem> cartItems = GetItems(userGuid);
            DeleteNullProducts(cartItems);
            AssignAvailablity(cartItems);
            cart.CartGuid = GetCartGuid(userGuid);
            cart.User = CurrentRequestData.CurrentUser;
            cart.UserGuid = userGuid;
            cart.Items = cartItems;
            cart.OrderEmail = GetOrderEmail(userGuid);
            cart.GiftMessage = GetGiftMessage(userGuid);
            cart.BillingAddressSameAsShippingAddress = _billingAddressSameAsShippingAddress.Get(cart, userGuid);
            cart.TermsAndConditionsRequired = _ecommerceSettings.TermsAndConditionsRequired;
            cart.TermsAndConditionsAccepted = GetTermsAndConditionsAccepted(userGuid);
            return cart;
        }

        private bool GetTermsAndConditionsAccepted(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<bool>(CartManager.TermsAndConditionsAcceptedKey, userGuid);
        }

        private void AssignAvailablity(List<CartItem> cartItems)
        {
            foreach (CartItem cartItem in cartItems)
            {
                CartItem item = cartItem;
                item.CanBuyStatus = _cartItemAvailablityService.CanBuy(item);
            }
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
}